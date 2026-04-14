using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Impl;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.ApiClient.OpenAi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class OpenAiClient : IOpenAiClient
    {
        private readonly IApiClient _apiClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://api.openai.com/v1";
        private const string EmbeddingModel = "text-embedding-3-small";
        private const string ChatModel = "gpt-4o-mini";
        private const int MaxInputChars = 8000;

        public OpenAiClient(IApiClient apiClient)
        {
            _apiClient = apiClient;
            _apiKey = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.OPENAI_API_KEY);
        }

        private List<KeyValuePair<string, string>> GetHeaders()
        {
            return new List<KeyValuePair<string, string>>
            {
                new("Authorization", $"Bearer {_apiKey}")
            };
        }

        private static string Truncar(string texto, int maxChars)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            return texto.Length <= maxChars ? texto : texto[..maxChars];
        }

        public async Task<RespostaServico<EmbeddingResult>> GerarEmbeddingAsync(string texto)
        {
            var body = new { model = EmbeddingModel, input = Truncar(texto, MaxInputChars) };
            var response = await _apiClient.PostAsync<OpenAiEmbeddingResponse>(body, $"{BaseUrl}/embeddings", GetHeaders());

            if (!response.Sucesso || response.Resposta?.Data == null || response.Resposta.Data.Count == 0)
            {
                return new RespostaServico<EmbeddingResult>
                {
                    Sucesso = false,
                    Mensagem = response.Mensagem ?? "Falha ao gerar embedding"
                };
            }

            return new RespostaServico<EmbeddingResult>
            {
                Sucesso = true,
                Resposta = new EmbeddingResult { Vetor = response.Resposta.Data[0].Embedding }
            };
        }

        public async Task<RespostaServico<List<EmbeddingResult>>> GerarEmbeddingsEmBatchAsync(List<string> textos)
        {
            var inputs = textos.Select(t => Truncar(t, MaxInputChars)).ToList();
            var body = new { model = EmbeddingModel, input = inputs };
            var response = await _apiClient.PostAsync<OpenAiEmbeddingResponse>(body, $"{BaseUrl}/embeddings", GetHeaders());

            if (!response.Sucesso || response.Resposta?.Data == null)
            {
                return new RespostaServico<List<EmbeddingResult>>
                {
                    Sucesso = false,
                    Mensagem = response.Mensagem ?? "Falha ao gerar embeddings em batch"
                };
            }

            var results = response.Resposta.Data
                .OrderBy(d => d.Index)
                .Select(d => new EmbeddingResult { Vetor = d.Embedding })
                .ToList();

            return new RespostaServico<List<EmbeddingResult>> { Sucesso = true, Resposta = results };
        }

        public async Task<RespostaServico<ChatCompletionResult>> ChatCompletionAsync(
            string systemPrompt, List<ChatMessage> historico, string mensagemUsuario)
        {
            var messages = new List<object>();
            messages.Add(new { role = "system", content = systemPrompt });

            if (historico != null)
            {
                foreach (var msg in historico)
                    messages.Add(new { role = msg.Role, content = msg.Content });
            }

            messages.Add(new { role = "user", content = Truncar(mensagemUsuario, MaxInputChars) });

            var body = new
            {
                model = ChatModel,
                messages,
                max_completion_tokens = 500,
                temperature = 0.4
            };

            var response = await _apiClient.PostAsync<OpenAiChatResponse>(body, $"{BaseUrl}/chat/completions", GetHeaders());

            if (!response.Sucesso || response.Resposta?.Choices == null || response.Resposta.Choices.Count == 0)
            {
                return new RespostaServico<ChatCompletionResult>
                {
                    Sucesso = false,
                    Mensagem = response.Mensagem ?? "Falha na chat completion"
                };
            }

            return new RespostaServico<ChatCompletionResult>
            {
                Sucesso = true,
                Resposta = new ChatCompletionResult { Conteudo = response.Resposta.Choices[0].Message?.Content }
            };
        }
    }

    #region OpenAI API response models

    public class OpenAiEmbeddingResponse
    {
        public List<OpenAiEmbeddingData> Data { get; set; }
    }

    public class OpenAiEmbeddingData
    {
        public int Index { get; set; }
        public List<double> Embedding { get; set; }
    }

    public class OpenAiChatResponse
    {
        public List<OpenAiChatChoice> Choices { get; set; }
    }

    public class OpenAiChatChoice
    {
        public OpenAiChatMessage Message { get; set; }
    }

    public class OpenAiChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    #endregion
}
