using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Threading.Tasks;

namespace Social.Domain.Interfaces.AtendimentoFourmakers
{
    public interface ICuradoriaService
    {
        Task<ApiGenericResult<CuradoriaLogResult>> AtualizarLogAsync(string id, AtualizarCuradoriaInput input, string codigoInternoColaborador, string nomeColaborador, int orgId);
    }
}
