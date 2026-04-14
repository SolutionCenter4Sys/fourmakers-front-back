using DataTransferObject.Domain.Projeto.ProjetoOrg;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projeto.Domain.Interfaces.Services
{
    public interface IProjetoOrgService
    {
        Task<IEnumerable<ProjetosOrgResult>> ListarProjetos(int orgId, int cursor, int limite, int codStatus, string nomeProjeto);
        Task<ProjetoOrgDetalhesDTO> CadastrarProjeto(ProjetoOrgDTO param, string cpf, int orgId);
        Task<ProjetoOrgDetalhesDTO> EditarProjeto(ProjetoOrgDTO param, string cpf, int orgId);
        Task<ProjetoOrgDetalhesDTO> ObterProjetoPorCodigo(string codProjeto, int orgId);
        Task<List<ProjetoOrgColaboradorGerenteDetalhesDTO>> ListarGestoresProjeto(string codigoDiretoria, string codigoDepartamento, string codigoGestorAdm, int orgId, string cpfRequest);
        Task<IEnumerable<ProjetoSimplesDTO>> ListarProjetosDoCliente(string codCliente, int orgId);
    }
}