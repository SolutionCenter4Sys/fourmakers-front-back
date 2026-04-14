using DataTransferObject.Domain.CRM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IColaboracaoBridgeClient
    {
        Task<CRMContatoResponsavelOutputDTO> BuscarContatoResponsavel(string crm, string token);
        Task<IEnumerable<VwClienteEnderecoDTO>> GetClientesFourmakersCrm();
        Task<IEnumerable<ContactDetailsDTO>> GetGestoresPorAccountNos(IEnumerable<string> accountNos);
        Task<IEnumerable<VwCotacoesIncluindoAccountNoDTO>> GetCotacoesIncluindoAccountNoByQuoteNos(IEnumerable<string> quoteNos);
    }
}