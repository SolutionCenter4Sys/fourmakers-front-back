using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Fourmakers;

namespace Foursys.Domain.Interfaces.Services;

public interface IFourmakersLeadsService
{
    Task<ApiGenericResult> InserirLeadAsync(string nome, string email, string telefone, string nomeEmpresa, CaputraLeadColaboradorQuantidadeEnum opcaoColaboradorEnum);
}