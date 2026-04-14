using DataTransferObject.Domain.Usuario.Permissao;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Usuario.Permissao
{
    public interface IGrupoAcessoRepository
    {
        Task<IEnumerable<GrupoAcessoDTO>> ListarGruposAcesso(int orgId);

        Task<IEnumerable<GrupoAcessoFuncionalidadeSistemaResult>> ListarGruposAcessoFuncionalidadesSistema(int orgId);
        Task AdicionarGrupoAcessoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId, string cpfAlterador);
        Task RemoverGrupoAcessoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId, string cpfAlterador);
        Task<bool> GrupoAcessoNaoPertenceOrg(int grupoAcessoId, int orgId);
        Task<bool> GrupoAcessoEstaRelacionadoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId);
        Task<int> CriarGrupoAcesso(string descricao, int orgId, string cpfAlterador, bool acessoTodosClientes);
        Task AtualizarGrupoAcesso(int grupoAcessoId, string descricao, string cpfAlterador, bool acessoTodosClientes);
        Task AdicionarClienteAoGrupoAcesso(int grupoAcessoId, string codigoCliente, int orgId, string cpfAlterador);
        Task RemoverTodosClientesDoGrupoAcesso(int grupoAcessoId, int orgId, string cpfAlterador);
        Task<bool> DescricaoGrupoAcessoJaExiste(string descricao, int orgId, int? grupoAcessoIdExcluir = null);
        Task<bool> ClienteExiste(string codigoCliente, int orgId);
        Task<List<string>> ListarTodosClientesAtivosPorOrg(int orgId);
        Task<IEnumerable<PessoaGrupoAcessoDTO>> ListarPessoasPorGrupoAcesso(int orgId, int? grupoId = null);
    }
}