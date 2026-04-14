using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Interfaces.Factorys
{
    public interface IUsuarioDomainFactory
    {
        IUsuarioModel buildUsuarioModel();
    }
}