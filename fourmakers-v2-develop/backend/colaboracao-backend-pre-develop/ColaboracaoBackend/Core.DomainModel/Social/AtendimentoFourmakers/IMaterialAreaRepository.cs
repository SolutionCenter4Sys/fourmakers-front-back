using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Core.Domain.Social.AtendimentoFourmakers
{
    public interface IMaterialAreaRepository
    {
        Task<IEnumerable<MaterialAreaResult>> ListarAsync(int orgId);
        Task<MaterialAreaResult> ObterPorIdAsync(string id, int orgId);
        Task<string> InserirAsync(MaterialAreaInsertInput input, int orgId, IDbTransaction transaction = null);
        Task<bool> AtualizarAsync(string id, MaterialAreaUpdateInput input, int orgId, IDbTransaction transaction = null);
        Task<bool> DeletarAsync(string id, int orgId, IDbTransaction transaction = null);
        Task<bool> SlugExisteAsync(string slug, int orgId, string ignorarId = null);
    }
}
