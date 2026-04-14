using ApiClient.Domain.Interfaces;
using Comunicacao.Infra;
using Core.Domain.Social.AtendimentoFourmakers;
using DataTransferObject.Domain.ApiClient.OpenAi;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using Logs.Infra.Attributes;
using Social.Domain.Interfaces.AtendimentoFourmakers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Social.Domain.Impl.AtendimentoFourmakers
{
    [LogDomainClass]
    public class WebhookWhatsAppService : IWebhookWhatsAppService
    {
        private readonly IEvolutionApiService _evolutionApiService;
        private readonly IOpenAiClient _openAiClient;
        private readonly IKbChunkRepository _kbChunkRepository;
        private readonly IAssistenteConfigRepository _configRepository;
        private readonly IChamadoRepository _chamadoRepository;
        private readonly IAssistenteChatLogRepository _chatLogRepository;
        private readonly IWhatsAppChamadoPromptRepository _promptRepository;

        private static readonly string[] PrefixosChamado =
            { "CHAMADO:", "abrir chamado:", "quero abrir chamado:", "registrar chamado:" };

        private static readonly HashSet<string> EventosIgnorados = new(StringComparer.OrdinalIgnoreCase)
            { "connection.update", "qrcode", "presence.update" };

        public WebhookWhatsAppService(
            IEvolutionApiService evolutionApiService,
            IOpenAiClient openAiClient,
            IKbChunkRepository kbChunkRepository,
            IAssistenteConfigRepository configRepository,
            IChamadoRepository chamadoRepository,
            IAssistenteChatLogRepository chatLogRepository,
            IWhatsAppChamadoPromptRepository promptRepository)
        {
            _evolutionApiService = evolutionApiService;
            _openAiClient = openAiClient;
            _kbChunkRepository = kbChunkRepository;
            _configRepository = configRepository;
            _chamadoRepository = chamadoRepository;
            _chatLogRepository = chatLogRepository;
            _promptRepository = promptRepository;
        }

        public async Task<ApiGenericResult<WebhookWhatsAppResult>> ProcessarWebhookAsync(object payload, string webhookSecret, int orgId)
        {
            var result = new ApiGenericResult<WebhookWhatsAppResult>();
            var webhookResult = new WebhookWhatsAppResult { Ok = true, Erros = new List<string>() };

            if (!_evolutionApiService.ValidarWebhookSecret(webhookSecret))
            {
                result.Sucesso = false;
                result.Mensagem = "Webhook secret inválido";
                return result;
            }

            var json = payload is JsonElement je ? je : JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(payload));

            var eventType = json.TryGetProperty("event", out var evt) ? evt.GetString() : null;
            if (eventType != null && EventosIgnorados.Contains(eventType))
            {
                result.Retorno = webhookResult;
                return result;
            }

            var mensagens = ExtrairMensagens(json);

            foreach (var (numero, pushName, texto) in mensagens)
            {
                webhookResult.Recebidas++;
                try
                {
                    var prefixoChamado = PrefixosChamado.FirstOrDefault(p =>
                        texto.StartsWith(p, StringComparison.OrdinalIgnoreCase));

                    if (prefixoChamado != null)
                    {
                        var conteudo = texto[prefixoChamado.Length..].Trim();
                        var linhas = conteudo.Split('\n', 2);
                        var assunto = Truncar(linhas[0].Trim(), 500);
                        var descricao = linhas.Length > 1 ? Truncar(linhas[1].Trim(), 8000) : null;
                        var label = Truncar(pushName, 120);
                        if (string.IsNullOrEmpty(label)) label = numero;

                        var chamadoId = await _chamadoRepository.InserirAsync(new ChamadoInsertInput
                        {
                            CodigoInternoColaborador = null,
                            CooperadoLabel = $"{label} ({numero})",
                            Assunto = assunto,
                            Descricao = descricao,
                            Status = "aberto",
                            Prioridade = "media"
                        }, orgId);

                        var resposta = $"Chamado registrado com sucesso!\nProtocolo: {chamadoId}\nAssunto: {assunto}";
                        await _evolutionApiService.EnviarMensagemTextoAsync(numero, resposta);

                        webhookResult.ChamadosCriados++;
                        webhookResult.Respondidas++;
                    }
                    else
                    {
                        var respostaChat = await ExecutarPipelineRag(texto, orgId);
                        var textoResposta = respostaChat.conteudo;

                        if (respostaChat.sugerirChamado)
                        {
                            textoResposta += "\n\n---\nPara abrir um chamado, envie uma mensagem começando com:\nCHAMADO: [seu assunto]";
                        }

                        await _evolutionApiService.EnviarMensagemTextoAsync(numero, textoResposta);
                        webhookResult.Respondidas++;
                    }
                }
                catch (Exception ex)
                {
                    webhookResult.Erros.Add($"Erro ao processar mensagem de {numero}: {ex.Message}");
                }
            }

            result.Retorno = webhookResult;
            return result;
        }

        private async Task<(string conteudo, bool sugerirChamado)> ExecutarPipelineRag(string mensagem, int orgId)
        {
            var config = await _configRepository.ObterAsync(orgId);
            var ragTopK = config?.RagTopK ?? 8;
            var ragMinSimilarity = config?.RagSimilaridadeMinima ?? 0.22;
            var ticketThreshold = 0.35;
            if (config?.LimiarSimilaridadeChamadoPercent > 0)
                ticketThreshold = config.LimiarSimilaridadeChamadoPercent / 100.0;

            var embeddingResponse = await _openAiClient.GerarEmbeddingAsync(mensagem);
            if (!embeddingResponse.Sucesso)
                return ("Desculpe, não consegui processar sua mensagem no momento.", true);

            var embeddingJson = JsonSerializer.Serialize(embeddingResponse.Resposta.Vetor);
            var chunks = (await _kbChunkRepository.BuscarPorSimilaridadeAsync(
                embeddingJson, orgId, ragTopK, ragMinSimilarity)).ToList();

            var sugerirChamado = chunks.Count == 0 || chunks.Max(c => c.Similaridade) < ticketThreshold;

            var nomeAssistente = config?.NomeAssistente ?? "Assistente";
            var contexto = chunks.Count > 0
                ? string.Join("\n---\n", chunks.Select(c => c.Conteudo))
                : "Nenhum material relevante encontrado.";

            var systemPrompt = $@"Você é {nomeAssistente}, um assistente virtual via WhatsApp.
Responda de forma concisa e direta, adequada para mensagens de WhatsApp.
Use o contexto abaixo para responder.

--- CONTEXTO ---
{contexto}
--- FIM DO CONTEXTO ---";

            var chatResponse = await _openAiClient.ChatCompletionAsync(
                systemPrompt, new List<ChatMessage>(), mensagem);

            if (!chatResponse.Sucesso)
                return ("Desculpe, não consegui gerar uma resposta no momento.", true);

            return (chatResponse.Resposta.Conteudo, sugerirChamado);
        }

        private static List<(string numero, string pushName, string texto)> ExtrairMensagens(JsonElement json)
        {
            var mensagens = new List<(string, string, string)>();

            if (!json.TryGetProperty("data", out var data))
                return mensagens;

            var fromMe = data.TryGetProperty("key", out var key) &&
                         key.TryGetProperty("fromMe", out var fm) && fm.GetBoolean();
            if (fromMe) return mensagens;

            var remoteJid = key.TryGetProperty("remoteJid", out var jid) ? jid.GetString() : null;
            if (string.IsNullOrEmpty(remoteJid) || remoteJid.Contains("@g.us"))
                return mensagens;

            var numero = Regex.Replace(remoteJid.Split('@')[0], @"[^\d]", "");
            var pushName = data.TryGetProperty("pushName", out var pn) ? pn.GetString() : null;
            if (pushName != null && pushName.Length > 120) pushName = pushName[..120];

            var texto = ExtrairTextoMensagem(data);
            if (!string.IsNullOrWhiteSpace(texto))
                mensagens.Add((numero, pushName, texto));

            return mensagens;
        }

        private static string ExtrairTextoMensagem(JsonElement data)
        {
            if (!data.TryGetProperty("message", out var message))
                return null;

            var atual = message;

            foreach (var wrapper in new[] { "ephemeralMessage", "viewOnceMessage", "viewOnceMessageV2" })
            {
                if (atual.TryGetProperty(wrapper, out var w) && w.TryGetProperty("message", out var inner))
                    atual = inner;
            }

            if (atual.TryGetProperty("conversation", out var conv))
                return conv.GetString();

            if (atual.TryGetProperty("extendedTextMessage", out var ext) && ext.TryGetProperty("text", out var extText))
                return extText.GetString();

            if (atual.TryGetProperty("imageMessage", out var img) && img.TryGetProperty("caption", out var imgCap))
                return imgCap.GetString();

            if (atual.TryGetProperty("videoMessage", out var vid) && vid.TryGetProperty("caption", out var vidCap))
                return vidCap.GetString();

            return null;
        }

        private static string Truncar(string texto, int maxChars)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            return texto.Length <= maxChars ? texto : texto[..maxChars];
        }
    }
}
