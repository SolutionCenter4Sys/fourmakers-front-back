using DataTransferObject.Domain;
using DataTransferObject.Domain.Usuario;
using System.Threading.Tasks;

namespace Usuario.Domain.Interfaces.Services
{
    public interface IAcessoUsuarioService
    {
        public Task<TipoAcessoEnum> EnviaTokenAcessoEmail(string email, int orgId, bool forceCodigoEmail = false);
        public Task<UsuarioResult> ValidaTokenAcessoEmail(string email, string token, int orgId);
        Task<UsuarioAcessoSemOrgResult> EnviaTokenAcessoEmailSemOrg(string email);
        Task<string> ObtemCodigoAcessoEmail(string tokenSistema, string email, int orgId);
    }
}