using Colaborador.Domain.Interfaces.Models;

namespace Colaborador.Domain.Interfaces.Factorys
{
    public interface IPaisDomainFactory
    {
        IPaisModel buildPaisModel();
        IPaisModel buildPaisModel(int id, string descricao);
    }
}