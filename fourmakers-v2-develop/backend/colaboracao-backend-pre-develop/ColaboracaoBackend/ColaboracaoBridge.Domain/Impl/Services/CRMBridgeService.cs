using ColaboracaoBridge.Domain.Interfaces.Services;
using Core.Domain;
using DataTransferObject.Domain.CRM;
using Logs.Infra.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ColaboracaoBridge.Domain.Impl.Services
{
    [LogDomainClass]
    public class CRMBridgeService : ICRMBridgeService
    {
        private readonly ICRMRepository _crmRepository;

        public CRMBridgeService(ICRMRepository crmRepository)
        {
            _crmRepository = crmRepository;
        }

        public CRMContatoResponsavelOutputDTO BuscarContatoResponsavel(string crm)
        {
            try
            {
                CRMContatoResponsavelOutputDTO retorno;
                crm = crm.ToUpper();

                if (crm.StartsWith("POT"))
                {
                    retorno = _crmRepository.BuscarContatoOportunidade(crm);
                }
                else
                {
                    var oportunidade = _crmRepository.BuscarOportunidade(crm);
                    retorno = _crmRepository.BuscarContatoOportunidade(oportunidade);
                }
                return retorno;
            }
            catch
            {
                throw;
            }
        }

        public Task<IEnumerable<VwClienteEnderecoDTO>> GetClientesCrm()
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<VwCotacoesDTO>> GetCotacoesByCrmId(string crmId)
        {
            return await _crmRepository.GetCotacoesByCrmId(crmId);
        }

        public async Task<IEnumerable<VwCotacoesDTO>> GetCotacoesLastTwoYearsByAccountId(int accountId)
        {
            return await _crmRepository.GetCotacoesLastTwoYearsByAccountId(accountId);
        }

        public async Task<IEnumerable<VwClienteEnderecoDTO>> GetClientesFourmakersCrm()
        {
            return await _crmRepository.GetClientesFourmakersCrm();
        }

        public async Task<IEnumerable<VwCotacoesIncluindoAccountNoDTO>> GetCotacoesIncluindoAccountNoByQuoteNos(List<string> quoteNos)
        {
            return await _crmRepository.GetCotacoesIncluindoAccountNoByQuoteNos(quoteNos);
        }

        public async Task<IEnumerable<ContactDetailsDTO>> GetGestoresPorAccountNos(IEnumerable<string> accountNos)
        {
            return await _crmRepository.GetGestoresPorAccountNos(accountNos);
        }
    }
}