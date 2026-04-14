using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Interfaces.GestaoDesempenho.Gestor
{
    public interface IGestaoDesempenhoGestorService
    {
        Task<ApiGenericResult<DashboardGestorDTO>> ObterDashboardGestorAsync(string codigoInternoColaboradorGestor, int orgId);
        Task<ApiGenericResult<MeusColaboradoresResponseDTO>> ObterMeusColaboradoresAsync(string codigoInternoColaboradorGestor, int orgId, MeusColaboradoresRequestDTO filtros);
        Task<ApiGenericResult<InserirFeedbackRequestDTO>> InserirFeedbackAsync(string codigoInternoColaboradorGestor, int orgId, InserirFeedbackRequestDTO request);
        Task<ApiGenericResult<InserirOneOnOneRequestDTO>> InserirOneOnOneAsync(string codigoInternoColaboradorGestor, int orgId, InserirOneOnOneRequestDTO request);
        Task<ApiGenericResult<DashboardColaboradorDTO>> ObterDashboardColaboradorAsync(string codigoInternoColaboradorGestor, string codigoInternoColaboradorAvaliado, int orgId);
        Task<ApiGenericResult<InserirPautaSugeridaGestorResponseDTO>> InserirPautaSugeridaGestorAsync(string codigoInternoColaboradorGestor, int orgId, InserirPautaSugeridaGestorRequestDTO request);
        Task<ApiGenericResult> AtualizarRegistroCriticoOneOnOneAsync(string codigoInternoColaboradorGestor, int orgId, string oneOnOneId, bool registroCritico);
    }
}
