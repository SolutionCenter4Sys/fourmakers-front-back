using Colaborador.Domain.Interfaces.Models;

namespace Colaborador.Domain.Interfaces.Factorys
{
    public interface INacionalidadeDomainFactory
    {
        INacionalidadeModel buildNacionalidadeModel();
        INacionalidadeModel buildNacionalidadeModel(int id, string descricao);
    }
}