using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Interfaces.Factorys
{
    public interface ITokenUsuarioAcessoDomainFactory
    {
        ITokenUsuarioAcessoModel buildTokenUsuarioAcessoModel();
        ITokenUsuarioAcessoModel buildTokenUsuarioAcessoModel(long id, long usuarioId, long tokenAcessoId);
    }
}