using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using System.Threading.Tasks;

namespace Social.Domain.Interfaces.AtendimentoFourmakers
{
    public interface IAuditoriaAtendimentoService
    {
        Task<ApiGenericResult<AuditoriaListaResult>> ListarAsync(FiltroAuditoriaInput filtro, int orgId);
        Task RegistrarAsync(AuditoriaInsertInput input, int orgId);
        Task<byte[]> ExportarCsvAsync(FiltroAuditoriaInput filtro, int orgId);
    }
}
