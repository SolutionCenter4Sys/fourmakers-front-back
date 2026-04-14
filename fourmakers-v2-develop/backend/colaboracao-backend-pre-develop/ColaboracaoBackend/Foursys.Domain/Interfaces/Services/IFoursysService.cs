using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Foursys;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foursys.Domain.Interfaces.Services
{
    public interface IFoursysService
    {
        BuscaCargoResult BuscarCargos(string busca, int cursor, int limite);
        BuscaDiretoriaResult BuscarDiretorias(string busca, int cursor, int limite);
        CargoDTO InsereCargo(string cargo);
        CargoDTO BuscarCargo(int id);
        CargoDTO AlteraCargo(int id, string cargo);
        bool DeletaCargo(int id, string cargo);
        List<UnidadesDTO> ListarUnidades();
        List<UsuarioAcessoDTO> ListarAcessoUsuarios();
        EnviarEmailCadastroIncompletoResult EnviarEmailCadastroIncompleto(IEnumerable<UsuarioAcessoDTO> usuarios);
        List<UnidadesDTO> ListarUnidadesPorOrg(int orgId);
        List<ColaboradoresTotaisOrgIdResult> ListaColaboradoresOrgId(int orgId, int page, int pageSize);
        Task<ListaUnidadesResult> ListarUnidadesPorOrgRestricao(int orgId, string cpfRequest);
    }
}
