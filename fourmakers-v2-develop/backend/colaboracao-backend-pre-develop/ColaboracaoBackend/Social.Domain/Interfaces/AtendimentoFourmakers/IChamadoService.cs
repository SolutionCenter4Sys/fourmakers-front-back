using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Social.Domain.Interfaces.AtendimentoFourmakers
{
    public interface IChamadoService
    {
        Task<ApiGenericResult<ChamadoResult>> CriarAsync(CriarChamadoInput input, string codigoInternoColaborador, string nomeColaborador, int orgId);
        Task<ApiGenericResult<ChamadoResult>> AtualizarAsync(string id, AtualizarChamadoInput input, string codigoInternoColaborador, string nomeColaborador, int orgId);
        Task<ApiGenericResult<List<ChamadoResult>>> ListarAsync(string codigoInternoColaborador, int orgId, string status = null);
        Task<ApiGenericResult<ChamadoResult>> ObterPorIdAsync(string id, string codigoInternoColaborador, int orgId);
        Task<ApiGenericResult<DistribuicaoStatusResult>> ObterDistribuicaoStatusAsync(int orgId);
    }
}
