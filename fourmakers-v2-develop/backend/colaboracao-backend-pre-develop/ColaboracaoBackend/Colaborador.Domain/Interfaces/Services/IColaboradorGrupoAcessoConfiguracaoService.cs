using DataTransferObject.Domain.Base;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IColaboradorGrupoAcessoConfiguracaoService
    {
        Task<ApiGenericResult> VerificarConfiguracaoGrupoAcesso(int orgId);
        Task AssociarGrupoAcessoPorConfiguracaoAsync(int tbOrgId, string codigoInternoColaborador, string cpfRequest);
    }
}