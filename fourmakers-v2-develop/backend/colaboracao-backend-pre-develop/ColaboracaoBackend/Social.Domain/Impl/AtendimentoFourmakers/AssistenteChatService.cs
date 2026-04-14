using ApiClient.Domain.Interfaces;
using Colaboracao.Helper;
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
using System.Threading.Tasks;

namespace Social.Domain.Impl.AtendimentoFourmakers
{
    [LogDomainClass]
    public class AssistenteChatService : IAssistenteChatService
    {
        private readonly IKbChunkRepository _kbChunkRepository;
        private readonly IAssistenteChatLogRepository _chatLogRepository;
        private readonly IAssistenteConfigRepository _configRepository;
        private readonly IOpenAiClient _openAiClient;

        public AssistenteChatService(
            IKbChunkRepository kbChunkRepository,
            IAssistenteChatLogRepository chatLogRepository,
            IAssistenteConfigRepository configRepository,
            IOpenAiClient openAiClient)
        {
            _kbChunkRepository = kbChunkRepository;
            _chatLogRepository = chatLogRepository;
            _configRepository = configRepository;
            _openAiClient = openAiClient;
        }

        public async Task<ApiGenericResult<ConversarResult>> ConversarAsync(ConversarInput input, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult<ConversarResult>();

            if (string.IsNullOrWhiteSpace(input?.Mensagem))
                throw new ArgumentException("Mensagem não pode ser vazia.");

            var mensagem = input.Mensagem.Trim();

            var config = await _configRepository.ObterAsync(orgId);
            var ragTopK = config?.RagTopK ?? ObterEnvInt("RAG_TOP_K", 8);
            var ragMinSimilarity = config?.RagSimilaridadeMinima ?? ObterEnvDouble("RAG_MIN_SIMILARITY", 0.22);
            var ticketThreshold = ObterEnvDouble("RAG_TICKET_SIM_THRESHOLD", 0.35);
            if (config?.LimiarSimilaridadeChamadoPercent > 0)
                ticketThreshold = config.LimiarSimilaridadeChamadoPercent / 100.0;

            if (ragTopK > 20) ragTopK = 20;

            var embeddingResponse = await _openAiClient.GerarEmbeddingAsync(mensagem);
            if (!embeddingResponse.Sucesso)
                throw new ApplicationException($"Falha ao gerar embedding da mensagem: {embeddingResponse.Mensagem}");

            var embeddingJson = JsonSerializer.Serialize(embeddingResponse.Resposta.Vetor);

            var chunks = (await _kbChunkRepository.BuscarPorSimilaridadeAsync(
                embeddingJson, orgId, ragTopK, ragMinSimilarity)).ToList();

            var sugerirChamado = chunks.Count == 0 ||
                (chunks.Max(c => c.Similaridade) < ticketThreshold);

            var nomeAssistente = config?.NomeAssistente ?? "Assistente";
            var instrucaoExtra = config?.InstrucaoSistemaExtra ?? "";

            var systemPrompt = MontarSystemPrompt(nomeAssistente, instrucaoExtra, chunks);

            var historico = FiltrarHistorico(input.Historico);

            var chatResponse = await _openAiClient.ChatCompletionAsync(systemPrompt, historico, mensagem);
            if (!chatResponse.Sucesso)
                throw new ApplicationException($"Falha na geração de resposta do assistente: {chatResponse.Mensagem}");

            var fontesJson = JsonSerializer.Serialize(chunks.Select(c => new
            {
                id = c.Id,
                titulo = c.Titulo,
                similaridade = Math.Round(c.Similaridade, 4),
                trecho = c.Conteudo?.Length > 500 ? c.Conteudo[..500] : c.Conteudo
            }));

            var logId = await _chatLogRepository.InserirAsync(new AssistenteChatLogInsertInput
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                MensagemUsuario = mensagem,
                MensagemAssistente = chatResponse.Resposta.Conteudo,
                Fontes = fontesJson,
                QtdChunks = chunks.Count,
                SimilaridadeMax = chunks.Count > 0 ? chunks.Max(c => c.Similaridade) : null,
                ExibirChamado = sugerirChamado
            }, orgId);

            result.Retorno = new ConversarResult
            {
                Conteudo = chatResponse.Resposta.Conteudo,
                SugerirChamado = sugerirChamado,
                LogId = logId
            };

            return result;
        }

        public async Task<ApiGenericResult<FeedbackChatResult>> RegistrarFeedbackAsync(FeedbackChatInput input, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult<FeedbackChatResult>();

            if (string.IsNullOrWhiteSpace(input?.LogId))
                throw new ArgumentException("LogId é obrigatório.");

            if (input.Feedback != "up" && input.Feedback != "down")
                throw new ArgumentException("Feedback deve ser 'up' ou 'down'.");

            var atualizado = await _chatLogRepository.AtualizarFeedbackAsync(
                input.LogId, codigoInternoColaborador, orgId, input.Feedback);

            if (!atualizado)
            {
                var log = await _chatLogRepository.ObterPorIdAsync(input.LogId, orgId);
                if (log == null || log.CodigoInternoColaborador != codigoInternoColaborador)
                    throw new InvalidOperationException("Turno não encontrado.");

                result.Retorno = new FeedbackChatResult
                {
                    Ok = false,
                    FeedbackJaEnviado = true,
                    FeedbackExistente = log.Feedback
                };
                return result;
            }

            result.Retorno = new FeedbackChatResult { Ok = true };
            return result;
        }

        private static string MontarSystemPrompt(string nomeAssistente, string instrucaoExtra, List<KbChunkSimilarResult> chunks)
        {
            var contexto = chunks.Count > 0
                ? string.Join("\n---\n", chunks.Select(c => c.Conteudo))
                : "Nenhum material relevante encontrado na base de conhecimento.";

            var prompt = $@"Você é {nomeAssistente}, um assistente virtual.
Responda com base EXCLUSIVAMENTE no contexto fornecido abaixo.
Se não souber a resposta, diga que não encontrou informações e sugira abrir um chamado.

{(string.IsNullOrEmpty(instrucaoExtra) ? "" : instrucaoExtra + "\n")}
--- CONTEXTO ---
{contexto}
--- FIM DO CONTEXTO ---";

            return prompt;
        }

        private static List<ChatMessage> FiltrarHistorico(List<HistoricoMensagemInput> historico)
        {
            if (historico == null || historico.Count == 0)
                return new List<ChatMessage>();

            var validRoles = new HashSet<string> { "user", "assistant" };
            return historico
                .Where(h => validRoles.Contains(h.Role) && !string.IsNullOrEmpty(h.Conteudo))
                .TakeLast(12)
                .Select(h => new ChatMessage { Role = h.Role, Content = h.Conteudo })
                .ToList();
        }

        public async Task<ApiGenericResult<List<AssistenteChatLogResult>>> ListarLogsAsync(FiltroLogsInput filtro, int orgId)
        {
            var result = new ApiGenericResult<List<AssistenteChatLogResult>>();
            var limite = filtro?.Limite ?? 200;
            if (limite > 500) limite = 500;
            var offset = filtro?.Offset ?? 0;

            var logs = await _chatLogRepository.ListarAsync(orgId, limite, offset, filtro?.StatusCuradoria, filtro?.Feedback);
            result.Retorno = logs.ToList();
            return result;
        }

        public async Task<ApiGenericResult<FeedbackStatsResult>> ObterFeedbackStatsAsync(int orgId)
        {
            var result = new ApiGenericResult<FeedbackStatsResult>();
            result.Retorno = await _chatLogRepository.ObterFeedbackStatsAsync(orgId);
            return result;
        }

        public async Task<ApiGenericResult<MetricasMensalResult>> ObterMetricasMensalAsync(int orgId)
        {
            var result = new ApiGenericResult<MetricasMensalResult>();
            result.Retorno = await _chatLogRepository.ObterMetricasMensalAsync(orgId);
            return result;
        }

        private static int ObterEnvInt(string key, int defaultValue)
        {
            var val = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(key);
            return int.TryParse(val, out var parsed) ? parsed : defaultValue;
        }

        private static double ObterEnvDouble(string key, double defaultValue)
        {
            var val = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(key);
            return double.TryParse(val, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out var parsed) ? parsed : defaultValue;
        }
    }
}
