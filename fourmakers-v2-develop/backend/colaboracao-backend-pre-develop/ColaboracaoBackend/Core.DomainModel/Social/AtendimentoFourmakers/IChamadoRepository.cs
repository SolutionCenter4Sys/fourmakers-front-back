using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Core.Domain.Social.AtendimentoFourmakers
{
    public interface IChamadoRepository
    {
        Task<ChamadoResult> ObterPorIdAsync(string id, int orgId);
        Task<IEnumerable<ChamadoResult>> ListarAsync(int orgId, string codigoInternoColaborador = null,
            string status = null, int? limite = null, int? offset = null);
        Task<string> InserirAsync(ChamadoInsertInput input, int orgId, IDbTransaction transaction = null);
        Task<bool> AtualizarAsync(string id, ChamadoUpdateInput input, int orgId, IDbTransaction transaction = null);
        Task<IEnumerable<DistribuicaoStatusItem>> ObterDistribuicaoStatusAsync(int orgId);
    }
}
