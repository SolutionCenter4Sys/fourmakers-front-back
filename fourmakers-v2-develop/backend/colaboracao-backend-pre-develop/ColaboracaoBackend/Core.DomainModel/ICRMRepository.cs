using DataTransferObject.Domain.CRM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain
{
    public interface ICRMRepository
    {
        string BuscarOportunidade(string cotacao);
        CRMContatoResponsavelOutputDTO BuscarContatoOportunidade(string oportunidade);
        Task<IEnumerable<ContactDetailsDTO>> GetGestoresPorAccountNos(IEnumerable<string> accountNos);
        Task<IEnumerable<VwClienteEnderecoDTO>> GetClientesCrm();
        Task<VwClienteEnderecoDTO> GetClientePorAccountId(string accountId);
        Task<IEnumerable<VwClienteEnderecoDTO>> GetClientesFourmakersCrm();
        Task<IEnumerable<VwCotacoesDTO>> GetCotacoesByCrmId(string crmId);
        Task<IEnumerable<VwCotacoesDTO>> GetCotacoesLastTwoYearsByAccountId(int accountId);
        Task<IEnumerable<VwCotacoesIncluindoAccountNoDTO>> GetCotacoesIncluindoAccountNoByQuoteNos(List<string> quoteNos);
    }
}