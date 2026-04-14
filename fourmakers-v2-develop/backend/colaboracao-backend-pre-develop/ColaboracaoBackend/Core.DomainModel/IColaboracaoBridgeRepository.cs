using DataTransferObject.Domain.CRM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain
{
    public interface IColaboracaoBridgeRepository
    {
        string BuscarOportunidade(string cotacao);
        CRMContatoResponsavelOutputDTO BuscarContatoOportunidade(string oportunidade);
        Task<IEnumerable<VwClienteEnderecoDTO>> GetClientesCrm();
        Task<IEnumerable<VwCotacoesDTO>> GetCotacoesByCrmId(string crmId);
        Task<IEnumerable<VwCotacoesDTO>> GetCotacoesLastTwoYearsByAccountId(int accountId);
    }
}