using System.Threading.Tasks;
using DataTransferObject.Domain.Fourmakers;

namespace Core.Domain.FourmakersLead;

public interface IFourmakersLeadsRepository
{
    Task InserirLeadAsync(string nome, string email, string telefone, string nomeEmpresa, CaputraLeadColaboradorQuantidadeEnum opcaoColaboradorEnum);
}