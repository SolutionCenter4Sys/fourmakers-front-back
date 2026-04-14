using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.RH;
using System.Threading.Tasks;

namespace GestaoPessoa.Domain.Interfaces.GestaoDesempenho.RH
{
    public interface IGestaoDesempenhoRHService
    {
        Task<ApiGenericResult<DashboardRHDTO>> ObterDashboardRHAsync(string cpfRequest, int orgId);
        Task<ApiGenericResult<ListaColaboradoresResponseDTO>> ObterListaColaboradoresAsync(string cpfRequest, int orgId, ListaColaboradoresRequestDTO filtros);
        Task<ApiGenericResult<InserirParametrizacaoRequestDTO>> InserirParametrizacaoAsync(string cpfRequest, int orgId, InserirParametrizacaoRequestDTO request);
        Task<ApiGenericResult<ParametrizacaoOrgDTO?>> ObterParametrizacaoPorOrgAsync(string cpfRequest, int orgId);
    }
}
