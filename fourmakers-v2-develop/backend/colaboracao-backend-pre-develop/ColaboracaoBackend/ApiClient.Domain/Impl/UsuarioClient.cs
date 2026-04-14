using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class UsuarioClient : IUsuarioClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private ILogger _log;
        private string _baseURL;

        public UsuarioClient(IApiClient apiClient, IConfiguration configuration, ILogger<UsuarioClient> logger)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.USUARIO_API_PATH);
            _log = logger;
        }

        public async Task<UsuarioResult> ConfirmaPrimeiroAcessoCandidato(string cpf, string fcmToken)
        {
            var baseUrl = _baseURL;
            var validarToken = _configuration["Clients:Usuario:ConfirmaPrimeiroAcessoCandidato"];
            var url = baseUrl + validarToken;
            var objeto = new { cpf = cpf, fcmToken = fcmToken };
            var headers = new List<KeyValuePair<string, string>>();

            var responseMessage = await _apiClient.PostAsync<UsuarioResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<bool> ValidaAcessoGrupoFuncionalidade(string cpf, string token, FuncionalidadeSistemaEnum enumFuncionalidade)
        {
            var baseUrl = _baseURL;
            var validarToken = _configuration["Clients:Usuario:ValidaAcessoGrupoFuncionalidade"];
            var url = baseUrl + validarToken;
            var objeto = new { cpf = cpf, funcionalidadeSistemaEnum = enumFuncionalidade };
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", token)
                    )
                };

            var responseMessage = await _apiClient.PostAsync<bool>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<bool> ValidarToken(string parameter)
        {
            var baseUrl = _baseURL;
            var validarToken = _configuration["Clients:Usuario:ValidarToken"];
            var objeto = new { token = parameter };
            var url = baseUrl + validarToken;
            var headers = new List<KeyValuePair<string, string>>();

            var responseMessage = await _apiClient.PostAsync<StatusResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Sucesso;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<UsuarioValidacaoResult> ValidarTokenSSO(string parameter)
        {
            var baseUrl = _baseURL;
            var validarToken = _configuration["Clients:Usuario:ValidarTokenSSO"];
            var objeto = new { token = parameter };
            var url = baseUrl + validarToken;
            var headers = new List<KeyValuePair<string, string>>();

            var responseMessage = await _apiClient.PostAsync<UsuarioValidacaoResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<UsuarioColaboradorDTO> ShowMe(string tokenUsuario)
        {
            var baseUrl = _baseURL;
            string listarTipoComentarios = _configuration["Clients:Usuario:ShowMe"];
            var url = baseUrl + listarTipoComentarios;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<UsuarioResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.usuario;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}