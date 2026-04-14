using ApiClient.Domain.Interfaces;

using CRM.Domain.Interfaces.Services;
using DataTransferObject.Domain.CRM;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace CRM.Domain.Impl.Services
{
    [LogDomainClass]
    public class CRMService : ICRMService
    {
        private readonly IColaboracaoBridgeClient _colaboracaoBridgeClient;

        public CRMService(IColaboracaoBridgeClient colaboracaoBridgeClient)
        {
            _colaboracaoBridgeClient = colaboracaoBridgeClient;
        }
        public async Task<CRMContatoResponsavelOutputDTO> BuscarContatoResponsavel(string crm, string token)
        {
            try
            {
                return await _colaboracaoBridgeClient.BuscarContatoResponsavel(crm, token);
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<VwClienteEnderecoDTO> GetClientesCrm()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<VwCotacoesDTO> GetCotacoesByCrmId(string crmId)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<VwCotacoesDTO> GetCotacoesLastTwoYearsByAccountId(int accountId)
        {
            throw new System.NotImplementedException();
        }
    }
}