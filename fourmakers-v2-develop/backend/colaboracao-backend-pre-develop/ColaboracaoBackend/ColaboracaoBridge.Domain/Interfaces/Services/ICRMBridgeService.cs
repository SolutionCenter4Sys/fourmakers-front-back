using DataTransferObject.Domain.CRM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColaboracaoBridge.Domain.Interfaces.Services
{
    public interface ICRMBridgeService
    {
        CRMContatoResponsavelOutputDTO BuscarContatoResponsavel(string crm);
        Task<IEnumerable<VwClienteEnderecoDTO>> GetClientesCrm();
        Task<IEnumerable<VwCotacoesDTO>> GetCotacoesByCrmId(string crmId);
        Task<IEnumerable<VwCotacoesDTO>> GetCotacoesLastTwoYearsByAccountId(int accountId);
        Task<IEnumerable<VwClienteEnderecoDTO>> GetClientesFourmakersCrm();
        Task<IEnumerable<ContactDetailsDTO>> GetGestoresPorAccountNos(IEnumerable<string> accountNos);
        Task<IEnumerable<VwCotacoesIncluindoAccountNoDTO>> GetCotacoesIncluindoAccountNoByQuoteNos(List<string> quoteNos);
    }
}