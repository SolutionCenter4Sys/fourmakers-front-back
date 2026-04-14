using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Foursys;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class FoursysClient : IFoursysClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;
        public FoursysClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.FOURSYS_API_PATH);
        }

        public async Task<ListaUnidadesResult> ListarUnidades(string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarUnidades = _configuration["Clients:Foursys:ListarUnidades"];
            var url = baseUrl + listarUnidades;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}",tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaUnidadesResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        public async Task<ListaUnidadesResult> ListarUnidadesPorOrg(string tokenUsuario, int orgId)
        {
            var baseUrl = _baseURL;
            var ListarUnidadesPorOrg = _configuration["Clients:Foursys:ListarUnidadesPorOrg"];
            var url = baseUrl + ListarUnidadesPorOrg + "?orgId=" + orgId;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}",tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaUnidadesResult>(url, headers);

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