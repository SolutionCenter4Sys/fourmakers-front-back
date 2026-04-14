using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Noticia;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class NoticiaClient : INoticiaClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public NoticiaClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.NOTICIA_API_PATH);
        }

        public async Task<List<NoticiaDTO>> BuscarNoticia(string busca, int cursor, int limite, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var buscarNoticia = _configuration["Clients:Noticia:BuscarNoticia"];
            var url = baseUrl + buscarNoticia + "?busca=" + busca + "&cursor=" + cursor + "&limite=" + limite;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaNoticiaResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Noticia;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}