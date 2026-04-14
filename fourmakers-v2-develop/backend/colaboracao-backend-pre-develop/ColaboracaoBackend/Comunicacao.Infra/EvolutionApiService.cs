using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comunicacao.Infra
{
    public class EvolutionApiService : IEvolutionApiService
    {
        private readonly IApiClient _apiClient;
        private readonly string _baseUrl;
        private readonly string _apiKey;
        private readonly string _instanceName;
        private readonly string _webhookSecret;

        public EvolutionApiService(IApiClient apiClient)
        {
            _apiClient = apiClient;
            _baseUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("EVOLUTION_API_BASE_URL");
            _apiKey = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("EVOLUTION_API_KEY");
            _instanceName = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("EVOLUTION_INSTANCE_NAME");
            _webhookSecret = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("EVOLUTION_WEBHOOK_SECRET");
        }

        public async Task<bool> EnviarMensagemTextoAsync(string numero, string texto)
        {
            if (string.IsNullOrEmpty(_baseUrl) || string.IsNullOrEmpty(_instanceName))
                return false;

            var url = $"{_baseUrl.TrimEnd('/')}/message/sendText/{_instanceName}";
            var headers = new List<KeyValuePair<string, string>>
            {
                new("apikey", _apiKey)
            };

            var body = new { number = numero, text = texto };
            var response = await _apiClient.PostAsync<object>(body, url, headers);
            return response.Sucesso;
        }

        public bool ValidarWebhookSecret(string secretRecebido)
        {
            if (string.IsNullOrEmpty(_webhookSecret))
                return true;

            return !string.IsNullOrEmpty(secretRecebido) && secretRecebido == _webhookSecret;
        }
    }
}
