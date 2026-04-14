using Colaborador.Domain.Interfaces.Models;

namespace Colaborador.Domain.Interfaces.Factorys
{
    public interface IFotoDomainFactory
    {
        IFotoModel buildFotoModel();
    }
}