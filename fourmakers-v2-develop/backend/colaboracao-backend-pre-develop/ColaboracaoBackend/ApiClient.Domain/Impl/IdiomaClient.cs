using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.SRS.Candidate;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class IdiomaClient : IIdiomaClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public IdiomaClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COMPETENCIA_API_PATH);
        }

        public async Task<List<SkillValuePair>> GetIdiomaInfoByDescricao(List<string> idiomas, string token)
        {
            var baseUrl = _baseURL;
            var endpoint = _configuration["Clients:Idioma:GetIdiomaInfoByDescricao"];
            var url = baseUrl + endpoint;

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<List<SkillValuePair>>(new { Idiomas = idiomas }, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<IdiomaColaboradorDTO>> ListarIdiomaColaborador(string cpf, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Idioma:ListarIdiomaColaborador"];
            var url = baseUrl + listarCompetencias + "?cpfColaborador=" + cpf;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaIdiomaColaboradorResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Idioma;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}