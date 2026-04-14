using Foursys.Domain.Interfaces.Models;

namespace Foursys.Domain.Interfaces.Factorys
{
    public interface IFoursysDomainFactory
    {
        IFoursysModel buildFoursysCargoModel(int id, string cargo, int totalResultCount, int filteredResultCount);
        IFoursysModel buildFoursysDiretoriaModel(string id, string diretoria, int totalResultCount);
        IFoursysModel buildFoursysCargoModel();
        IFoursysModel buildFoursysDiretoriaModel();
        IFoursysModel buildListarUnidades();
        IFoursysModel buildListarUnidades(string id, string descricao);
    }
}