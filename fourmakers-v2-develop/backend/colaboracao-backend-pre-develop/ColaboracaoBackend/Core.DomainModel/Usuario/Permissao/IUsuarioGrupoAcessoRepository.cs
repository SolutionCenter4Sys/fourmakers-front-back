using DataTransferObject.Domain.Usuario.Permissao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Usuario.Permissao
{
    public interface IUsuarioGrupoAcessoRepository
    {
        Task<IEnumerable<UsuarioGrupoAcessoResult>> ObterGruposAcessoPorUsuario(int usuarioId, int orgId);
        Task AdicionarUsuarioGrupoAcesso(int usuarioId, int grupoAcessoId, string cpfAlterador = null);
        Task RemoverUsuarioGrupoAcesso(int usuarioId, int grupoAcessoId, string cpfAlterador = null);
        Task<bool> UsuarioNaoPertenceOrg(int usuarioId, int orgId);
        Task<bool> UsuarioEstaRelacionadoGrupoAcesso(int usuarioId, int grupoAcessoId);
    }
}