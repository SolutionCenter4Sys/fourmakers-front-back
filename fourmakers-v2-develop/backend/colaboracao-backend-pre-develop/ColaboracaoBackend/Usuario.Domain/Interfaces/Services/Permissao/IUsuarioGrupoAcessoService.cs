using DataTransferObject.Domain.Usuario.Permissao;
using System.Threading.Tasks;

namespace Usuario.Domain.Interfaces.Services.Permissao
{
    public interface IUsuarioGrupoAcessoService
    {
        Task<UsuarioGrupoAcessoDTO> ObterGruposAcessoPorUsuario(int usuarioId, string cpfRequest, int orgId);
        Task<UsuarioGrupoAcessoFuncionalidadeSistemaDTO> ObterGruposAcessoFuncionalidadesSistemaPorUsuario(int usuarioId, string cpfRequest, int orgId);
        Task AdicionarUsuarioGrupoAcesso(int usuarioId, int grupoAcessoId, string cpfRequest, int orgId);
        Task RemoverUsuarioGrupoAcesso(int usuarioId, int grupoAcessoId, string cpfRequest, int orgId);
    }
}