using Colaborador.Domain.Interfaces.Models;

namespace Colaborador.Domain.Interfaces.Factorys
{
    public interface ISimpleColaboradorDomainFactory
    {
        ISimpleColaboradorModel buildSimpleColaboradorModel();
        ISimpleColaboradorModel buildSimpleColaboradorModel(string cpf, string nome_completo);
    }
}