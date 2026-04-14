using Usuario.Domain.Interfaces.Models;

namespace Usuario.Domain.Interfaces.Factorys
{
    public interface ITokenSistemaDomainFactory
    {
        ITokenSistemaModel buildTokenSistemaModel();
    }
}