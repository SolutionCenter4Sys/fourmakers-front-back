using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Foursys;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public interface IFoursysDtoRepository
    {
        CargoDTO BuscarCargo(int id);
        BuscaCargoResult BuscarCargos(string busca, int cursor, int limite);
        BuscaDiretoriaResult BuscarDiretorias(string busca, int cursor, int limite);
        CargoDTO InsereCargo(string cargo);
        CargoDTO AlteraCargo(int id, string cargo);
        void DeletaCargo(int id);
        List<UnidadesDTO> ListarUnidades();
        List<UnidadesDTO> ListarUnidadesPorOrg(int orgId);
        Task<List<UnidadesDTO>> ListarUnidadesPorOrgIdComRestricao(int orgId, List<string> diretorias);
    }
}
