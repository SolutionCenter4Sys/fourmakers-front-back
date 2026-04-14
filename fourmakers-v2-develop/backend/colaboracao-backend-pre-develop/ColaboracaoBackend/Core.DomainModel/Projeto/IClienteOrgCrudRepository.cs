using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Projeto.ClienteOrgCrud;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel.Projeto
{
    public interface IClienteOrgCrudRepository
    {
        Task<IEnumerable<ClienteOrgCrudResult>> ListarClienteOrgCrudsAsync(int orgId);
        Task<List<string>> ListarCodigosDosProjetosAssociadosAoClienteAsync(string codCliente, int orgId);
        Task<ClienteOrgCrudResult> ObterClienteOrgCrudPorIdAsync(Guid id, int orgId);
        Task<ClienteOrgCrudResult> ObterClienteOrgCrudPorCodigoAsync(string codigoParametro, int orgId);
        Task<ClienteOrgCrudResult> ObterClienteOrgCrudPorNomeAsync(string nome, int orgId);
        Task<ClienteOrgCrudResult> InserirClienteOrgCrudAsync(ClienteOrgCrudInput clienteOrgCrudInput, TipoCadastroClienteEnum tipoCadastroEnum);
        Task<ClienteOrgCrudResult> AtualizarClienteOrgCrudAsync(ClienteOrgCrudInput clienteOrgCrudInput, TipoCadastroClienteEnum tipoCadastroEnum);
        Task<bool> DeletarClienteOrgCrudAsync(Guid id, string codigoInternoColaborador, int orgId);
    }
}