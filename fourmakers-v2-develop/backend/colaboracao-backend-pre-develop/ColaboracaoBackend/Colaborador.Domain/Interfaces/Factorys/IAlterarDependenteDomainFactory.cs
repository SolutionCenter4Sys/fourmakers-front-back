using Colaborador.Domain.Interfaces.Models;

namespace Colaborador.Domain.Interfaces.Factorys
{
    public interface IAlterarDependenteDomainFactory
    {
        IDependenteModel buildDependenteModel();
        IDependenteModel buildDependenteModel(long registroUser);
    }
}