using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class SkillDesconhecidaClient : ISkillDesconhecidaClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public SkillDesconhecidaClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COMPETENCIA_API_PATH);
        }

        public async Task<List<KeyValuePair<string, long>>> GetSkillDesconhecidaInfoByDescricao(List<string> skills, string token)
        {
            var baseUrl = _baseURL;
            var endpoint = _configuration["Clients:SkillDesconhecida:GetSkillDesconhecidaInfoByDescricao"];
            var url = baseUrl + endpoint;

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<List<KeyValuePair<string, long>>>(new { Skills = skills }, url, headers);

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