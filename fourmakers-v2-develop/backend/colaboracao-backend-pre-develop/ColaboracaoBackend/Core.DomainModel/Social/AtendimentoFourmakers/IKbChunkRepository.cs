using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Core.Domain.Social.AtendimentoFourmakers
{
    public interface IKbChunkRepository
    {
        Task<IEnumerable<KbChunkSimilarResult>> BuscarPorSimilaridadeAsync(
            string embeddingJson, int orgId, int topK, double minSimilarity);
        Task<int> InserirEmBatchAsync(IEnumerable<KbChunkInsertInput> chunks, int orgId, IDbTransaction transaction = null);
        Task<int> DeletarPorFonteAsync(string fonteId, int orgId, IDbTransaction transaction = null);
        Task<int> ContarAsync(int orgId);
        Task<IEnumerable<KbChunkResumoResult>> ListarPorFonteAsync(string fonteId, int orgId);
    }
}
