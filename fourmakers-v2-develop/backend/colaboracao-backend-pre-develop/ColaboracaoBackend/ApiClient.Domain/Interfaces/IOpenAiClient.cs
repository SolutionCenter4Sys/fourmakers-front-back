using ApiClient.Infra.Impl;
using DataTransferObject.Domain.ApiClient.OpenAi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IOpenAiClient
    {
        Task<RespostaServico<EmbeddingResult>> GerarEmbeddingAsync(string texto);
        Task<RespostaServico<List<EmbeddingResult>>> GerarEmbeddingsEmBatchAsync(List<string> textos);
        Task<RespostaServico<ChatCompletionResult>> ChatCompletionAsync(
            string systemPrompt, List<ChatMessage> historico, string mensagemUsuario);
    }
}
