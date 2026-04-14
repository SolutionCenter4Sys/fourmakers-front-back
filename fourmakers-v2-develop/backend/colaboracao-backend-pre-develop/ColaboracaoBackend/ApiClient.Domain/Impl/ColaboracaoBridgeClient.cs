using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.CRM;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class ColaboracaoBridgeClient : IColaboracaoBridgeClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private ILogger _log;
        private string _baseURL;
        public ColaboracaoBridgeClient(IApiClient apiClient, IConfiguration configuration, ILogger<UsuarioClient> logger)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _log = logger;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BRIDGE_API_PATH);
        }

        public async Task<CRMContatoResponsavelOutputDTO> BuscarContatoResponsavel(string crm, string token)
        {
            var baseUrl = _baseURL;
            string buscarContato = "CRMBridge/BuscarContatoResponsavel";
            var url = baseUrl + buscarContato + "?crm=" + crm;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", token)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<CRMContatoResponsavelOutputDTO>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<IEnumerable<VwClienteEnderecoDTO>> GetClientesFourmakersCrm()
        {
            var baseUrl = _baseURL;
            string crmBridgeClientesFourmakersCrm = "CRMBridge/GetClientesFourmakersCrm";
            var url = baseUrl + crmBridgeClientesFourmakersCrm;
            var headers = new List<KeyValuePair<string, string>>();

            var responseMessage = await _apiClient.GetAsync<IEnumerable<VwClienteEnderecoDTO>>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<IEnumerable<ContactDetailsDTO>> GetGestoresPorAccountNos(IEnumerable<string> accountNos)
        {
            var baseUrl = _baseURL;
            string buscarContato = "CRMBridge/GetGestoresPorAccountNos";
            var url = $"{baseUrl}{buscarContato}";

            var headers = new List<KeyValuePair<string, string>>();

            var responseMessage = await _apiClient.PostAsync<IEnumerable<ContactDetailsDTO>>(accountNos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<IEnumerable<VwCotacoesIncluindoAccountNoDTO>> GetCotacoesIncluindoAccountNoByQuoteNos(IEnumerable<string> quoteNos)
        {
            var baseUrl = _baseURL;
            string buscarCotacoes = "CRMBridge/GetCotacoesIncluindoAccountNoByQuoteNos";
            var url = $"{baseUrl}{buscarCotacoes}";

            var headers = new List<KeyValuePair<string, string>>();

            var responseMessage = await _apiClient.PostAsync<IEnumerable<VwCotacoesIncluindoAccountNoDTO>>(quoteNos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}