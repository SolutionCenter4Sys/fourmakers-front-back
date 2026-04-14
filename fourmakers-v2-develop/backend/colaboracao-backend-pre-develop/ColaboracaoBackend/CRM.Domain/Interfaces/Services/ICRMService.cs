using DataTransferObject.Domain.CRM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Domain.Interfaces.Services
{
    public interface ICRMService
    {
        Task<CRMContatoResponsavelOutputDTO> BuscarContatoResponsavel(string crm, string token);
        IEnumerable<VwClienteEnderecoDTO> GetClientesCrm();
        IEnumerable<VwCotacoesDTO> GetCotacoesByCrmId(string crmId);
        IEnumerable<VwCotacoesDTO> GetCotacoesLastTwoYearsByAccountId(int accountId);
    }
}