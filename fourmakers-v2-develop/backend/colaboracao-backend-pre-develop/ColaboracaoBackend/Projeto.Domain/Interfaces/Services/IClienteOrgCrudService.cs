using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Projeto.ClienteOrgCrud;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projeto.Domain.Interfaces.Services
{
    public interface IClienteOrgCrudService
    {
        Task<ApiGenericResult<IEnumerable<ClienteOrgCrudResult>>> ListarClienteOrgCruds(string cpfRequest, int orgId);
        Task<ApiGenericResult<ClienteOrgCrudResult>> ObterClienteOrgCrudPorId(Guid id, string CpfRequest, int orgId);
        Task<ApiGenericResult<ClienteOrgCrudResult>> InserirClienteOrgCrud(ClienteOrgCrudInput clienteOrgCrudInput, string cpfRequest, int orgId);
        Task<ApiGenericResult<ClienteOrgCrudResult>> AtualizarClienteOrgCrud(ClienteOrgCrudInput clienteOrgCrudInput, Guid id, string cpfRequest, int orgId);
        Task<ApiGenericResult> DeletarClienteOrgCrud(Guid id, string cpfRequest, int orgId);
    }
}