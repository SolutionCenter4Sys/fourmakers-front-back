using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Comentario;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class ComentarioClient : IComentarioClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public ComentarioClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COMENTARIO_API_PATH);
        }

        public async Task<ComentarioDTO> InserirComentario(string cpf, int type, string texto, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var inserirComentario = _configuration["Clients:Comentario:InserirComentario"];
            var url = baseUrl + inserirComentario;
            var objeto = new { cpfColaborador = cpf, type = type, texto = texto };
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.PostAsync<ComentarioResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Comentario;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<ComentarioTipoDTO>> ListarTipoComentarios(string tokenUsuario)
        {
            var baseUrl = _baseURL;
            string listarTipoComentarios = _configuration["Clients:Comentario:ListarTipoComentarios"];
            var url = baseUrl + listarTipoComentarios;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaComentarioTipoResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.ComentarioTipoDTO;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}