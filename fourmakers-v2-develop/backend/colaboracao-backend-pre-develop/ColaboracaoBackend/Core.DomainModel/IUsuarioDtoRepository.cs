using DataTransferObject.Domain.Usuario;

namespace Core.DomainModel
{
    public interface IUsuarioDtoRepository
    {
        UsuarioDTO GetUserByCpf(string cpf);
        UsuarioDTO GetUserByCpfAndOrg(string cpf, int orgId);
        UsuarioDTO GetUserByLogin(string login, int orgId = 0);
        UsuarioDTO GetUserByLoginAndOrg(string login, int orgId);
        UsuarioDTO SaveUser(UsuarioDTO model);
        UsuarioDTO UpdateUser(UsuarioDTO model);
        void Logout(string login);
    }
}
