using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Threading.Tasks;

namespace Social.Domain.Interfaces.AtendimentoFourmakers
{
    public interface IAssistenteConfigService
    {
        Task<ApiGenericResult<AssistenteConfigResult>> ObterAsync(int orgId);
        Task<ApiGenericResult<AssistenteConfigResult>> AtualizarAsync(AtualizarAssistenteConfigInput input, string codigoInternoColaborador, string nomeColaborador, int orgId);
    }
}
