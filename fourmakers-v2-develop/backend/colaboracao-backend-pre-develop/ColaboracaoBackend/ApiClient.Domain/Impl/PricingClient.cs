using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Pricing.Equipe;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class PricingClient : IPricingClient
    {
        private readonly IApiClient _apiClient;

        public PricingClient(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IEnumerable<RatecardClienteResult>> GetRatecardByPropostas(IEnumerable<string> propostas)
        {
            string baseUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.PRICING_API_URL_BASE)?.TrimEnd('/');
            string pricingGetRatecardByPropostasUrl = $"{baseUrl}/Equipe/GetRatecardByPropostas";

            var response = await _apiClient.PostAsync<ResponseWrapper<IEnumerable<RatecardClienteResult>>>(
                propostas,
                pricingGetRatecardByPropostasUrl,
                new List<KeyValuePair<string, string>>()
            );

            if (response.Sucesso && response.Resposta != null)
            {
                return response.Resposta.Objeto;
            }
            else
            {
                return new List<RatecardClienteResult>();
            }
        }
    }

    public class ResponseWrapper<T>
    {
        public bool Resultado { get; set; }
        public string Mensagem { get; set; }
        public T Objeto { get; set; }
    }
}