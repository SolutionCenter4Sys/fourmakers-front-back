using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Foursys;

namespace Core.Domain
{
    public interface IFoursysRepository<TModel, TFactory>
    {
        List<TModel> BuscarCargos(string busca, int cursor, int limite, TFactory factory);
        List<TModel> BuscarDiretorias(string busca, int cursor, int limite, TFactory factory);
        List<TModel> ListarUnidades(TFactory factory);

        TModel InsereCargo(TModel model);
        TModel BuscarCargo(TModel model);
        TModel AlteraCargo(TModel model);
        TModel DeletaCargo(TModel model);
        List<TModel> ListarUnidadesPorOrg(TFactory factory, int orgId);
        Task<List<UnidadesDTO>> ListarUnidadesPorOrgIdComRestricao(int orgId, List<string>? diretorias);
    }
}