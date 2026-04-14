using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Projeto.ClienteOrgCrud;
using System.Threading.Tasks;

namespace Projeto.Domain.Interfaces.Services
{
    public interface IClienteOrgCrudValidatorService
    {
        Task ValidaClienteOrgCrud(ClienteOrgCrudInput clienteOrgCrudInput, CRUDEnum create);
    }
}