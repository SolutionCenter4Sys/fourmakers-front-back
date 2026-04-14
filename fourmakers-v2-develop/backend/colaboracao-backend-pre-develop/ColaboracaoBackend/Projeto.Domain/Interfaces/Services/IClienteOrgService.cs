using DataTransferObject.Domain.Projeto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projeto.Domain.Interfaces.Services
{
    public interface IClienteOrgService
    {
        Task<List<ClienteOrgDTO>> ListarClientesOrg(int orgId, string codigoClienteFiltro, string codigoGerenteProjeto);
        ClienteOrgDTO ObterClientePorCodigo(string codCliente, int orgId);
        Task<ClienteOrgDTO> ObterClientePorCodigoAsync(string codCliente, int orgId);
        Task AtivarClienteCasoEstejaAssociadoAAlgumProjetoAsync(string codCliente, int orgId);
        Task InativarClienteCasoNaoEstejaAssociadoANenhumProjetoAsync(string codCliente, int orgId);
    }
}