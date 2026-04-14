using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class FirebaseClient : IFirebaseClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public FirebaseClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.FIREBASE_API_PATH);
        }

        public async Task<StatusResult> EnviaPush(string token, string titulo, string mensagem)
        {
            var baseUrl = _baseURL;
            var enviaPush = _configuration["Clients:Firebase:EnviaPush"];

            var objeto = new
            {
                token = token,
                titulo = titulo,
                mensagem = mensagem
            };

            var url = baseUrl + enviaPush;
            var headers = new List<KeyValuePair<string, string>>();

            var responseMessage = await _apiClient.PostAsync<StatusResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                return responseMessage.Resposta;
                //throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<StatusResult> EnviaPushEmLote(List<string> tokens, string titulo, string mensagem)
        {
            var baseUrl = _baseURL;
            var enviaPushEmLote = _configuration["Clients:Firebase:EnviaPushEmLote"];

            var objeto = new
            {
                tokens = tokens,
                titulo = titulo,
                mensagem = mensagem
            };

            var url = baseUrl + enviaPushEmLote;
            var headers = new List<KeyValuePair<string, string>>();

            var responseMessage = await _apiClient.PostAsync<StatusResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                return responseMessage.Resposta;
                //throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}