using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Endosso;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NewApiAppColaboracao.Models.BI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class EndossoClient : IEndossoClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;
        private ILogger _log;

        public EndossoClient(IApiClient apiClient, IConfiguration configuration, ILogger<ColaboradorClient> logger)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.ENDOSSO_API_PATH);
            _log = logger;
        }

        public async Task<List<PedidoEndossoDTO>> ListarEndossosColaborador(string cpf, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarEndossos = _configuration["Clients:Endosso:ListarEndossosPorColaborador"];
            var url = baseUrl + listarEndossos + "?cpf=" + cpf;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<List<PedidoEndossoDTO>>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<InfoEndossoGrafico> ListarEndossosPorCompetencia(long id, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarEndossos = _configuration["Clients:Endosso:ListarEndossosPorCompetencia"];
            var url = baseUrl + listarEndossos + "?id=" + id;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<GraficoCompetenciaEndossoDTO>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.CompetenciaGraf.EndossoGraf;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}