using DataTransferObject.Domain.Pricing.Equipe;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IPricingClient
    {
        Task<IEnumerable<RatecardClienteResult>> GetRatecardByPropostas(IEnumerable<string> propostas);
    }
}