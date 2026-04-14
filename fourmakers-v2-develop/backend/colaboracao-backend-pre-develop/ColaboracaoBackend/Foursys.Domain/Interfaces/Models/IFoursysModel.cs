using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Foursys;
using Foursys.Domain.Interfaces.Factorys;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foursys.Domain.Interfaces.Models
{
    public interface IFoursysModel
    {
        int FilteredResultCount { get; set; }
        int TotalResultCount { get; set; }
        CargoDTO CargoDTO { get; set; }
        UnidadesDTO UnidadesDTO { get; set; }
        DiretoriaDTO DiretoriaDTO { get; set; }
        List<IFoursysModel> BuscarCargos(string busca, int cursor, int limite, IFoursysDomainFactory factory);
        List<IFoursysModel> BuscarDiretorias(string busca, int cursor, int limite, IFoursysDomainFactory factory);
        IFoursysModel InsereCargo(string cargo);
        IFoursysModel BuscarCargo(int id);
        IFoursysModel AlteraCargo(int id, string cargo);
        void DeletaCargo(int id, string cargo);
        List<IFoursysModel> ListarUnidades(IFoursysDomainFactory factory);
        List<IFoursysModel> ListarUnidadesPorOrg(IFoursysDomainFactory factory, int orgId);
        Task<List<UnidadesDTO>> ListarUnidadesPorOrgIdComRestricao(int orgId, List<string>? diretorias);
    }
}