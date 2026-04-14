using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Colaborador;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Interfaces.GestaoDesempenho.Colaborador
{
    public interface IGestaoDesempenhoColaboradorService
    {
        Task<ApiGenericResult<ColaboradorDashboardDTO>> ObterMeuDashboardAsync(string codigoInternoColaborador);
        Task<ApiGenericResult<string>> InserirVisualizacaoFeedbackAsync(string feedbackId, string codigoInternoColaborador);
        Task<ApiGenericResult<string>> InserirVisualizacaoOneOnOneAsync(string oneOnOneId, string codigoInternoColaborador);
        Task<ApiGenericResult<InserirPautaSugeridaColaboradorResponseDTO>> InserirPautaSugeridaColaboradorAsync(string codigoInternoColaboradorAvaliado, InserirPautaSugeridaColaboradorRequestDTO request);
    }
}
