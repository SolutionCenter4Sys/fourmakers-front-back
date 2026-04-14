using DataTransferObject.Domain.Usuario;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IUsuarioClient
    {
        Task<bool> ValidarToken(string parameter);
        Task<UsuarioValidacaoResult> ValidarTokenSSO(string parameter);
        Task<UsuarioResult> ConfirmaPrimeiroAcessoCandidato(string cpf, string fcmToken);
        Task<bool> ValidaAcessoGrupoFuncionalidade(string cpf, string token, FuncionalidadeSistemaEnum enumFuncionalidade);
        Task<UsuarioColaboradorDTO> ShowMe(string token);
    }
}