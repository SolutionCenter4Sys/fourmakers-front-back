using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Projeto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel.Projeto
{
    public interface IClienteOrgRepository
    {
        bool CadastrarClienteOrg(string codigoCliente, string nomeCliente, int orgId, bool ativo, bool clienteOcultoNaGestaoAlocados, TipoCadastroClienteEnum tipoCadastro);
        Task UpsertClienteOrg(ClienteOrgDTO cliente, int orgId, bool deveOcultarNaGestaoDeAlocados, TipoCadastroClienteEnum tipoCadastro);
        Task<List<ClienteOrgDTO>> ListarClientesOrg(int orgId, string codigoClienteFiltro, string codigoGerenteProjeto);
        List<ClienteOrgDTO> ListarClientesPorNome(string nomeCliente, int orgId);
        void InativarCliente(string codCliente, int orgId);
        void AtivarCliente(string codCliente, int orgId);
        ClienteOrgDTO ObterClientePorCodigo(string codCliente, int orgId);
        List<string> ListarCodigosDosProjetosAssociadosAoCliente(string codCliente, int orgId);
        Task<List<ClienteSumarioDTO>> ListarClienteSumario(int orgId);
        Task DesativarClientesQueForamSubstituidosPorCodigoClienteDoCRM(int orgId);
        Task<ClienteOrgDTO> ObterClientePorCodigoAsync(string codCliente, int orgId);
        Task<List<string>> ListarCodigosDosProjetosAssociadosAoAsync(string codCliente, int orgId);
        Task AtivarClienteAsync(string codCliente, int orgId);
        Task InativarClienteAsync(string codCliente, int orgId);

        Task<bool> CadastrarClienteOrgAsync(string codigoCliente, string nomeCliente, int orgId, bool ativo, bool clienteOcultoNaGestaoAlocados, TipoCadastroClienteEnum tipoCadastro);
    }
}