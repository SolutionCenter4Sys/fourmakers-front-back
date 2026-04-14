using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.CCH;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class CCHClient : ICCHClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private ILogger _log;
        private string _baseURL;
        private string _baseURL_Validacao;
        private string _baseURL_Hierarquia;
        private string _username;
        private string _password;
        private string _username_mapa_alocacao;
        private string _password_mapa_alocacao;
        private string _baseURL_mapa_alocacao;
        private string _baseURL_token_mapa_alocacao;

        public CCHClient(IApiClient apiClient, IConfiguration configuration, ILogger<UsuarioClient> logger)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CCH_BASE_URL_API);
            _baseURL_Validacao = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CCH_BASE_URL_VALIDACAO);
            _baseURL_Hierarquia = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_API_HIERARQUIA);
            _username = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CCH_USER_NAME);
            _password = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CCH_PASSWORD);
            _password_mapa_alocacao = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CCH_PASSWORD_MAPA_ALOCACAO);
            _username_mapa_alocacao = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CCH_USER_NAME_MAPA_ALOCACAO);
            _baseURL_mapa_alocacao = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CCH_BASE_URL_API_MAPA_ALOCACAO);
            _baseURL_token_mapa_alocacao = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CCH_BASE_URL_TOKEN_MAPA_ALOCACAO);

            _log = logger;
        }

        public async Task<TokenCCH> Autenticacao()
        {
            var baseUrl = _baseURL;
            var validarToken = "/Autenticacao.Auth/api/Auth/Token";
            var url = baseUrl + validarToken;
            var objeto = new { username = _username, password = _password };
            var headers = new List<KeyValuePair<string, string>>();

            var responseMessage = await _apiClient.PostAsync<TokenCCH>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        public async Task<TokenCCH> AutenticacaoMapaAlocacao()
        {
            var baseUrl = _baseURL_token_mapa_alocacao;
            var validarToken = "/Autenticacao.Auth/api/Auth/Token";
            var url = baseUrl + validarToken;
            var objeto = new { username = _username_mapa_alocacao, password = _password_mapa_alocacao };
            var headers = new List<KeyValuePair<string, string>>();

            var responseMessage = await _apiClient.PostAsync<TokenCCH>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<ColaboradorCCH>> Validacao(string emailColaborador, string tokenUsuario)
        {
            var baseUrl = _baseURL_Validacao;
            var validarToken = "/Validacao.API/api/Validacao";
            var url = baseUrl + validarToken + "?NmEnderecoEletronico=" + emailColaborador;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<List<ColaboradorCCH>>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<RecursoCCH> Recurso(string codigoProfissional, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var validarToken = "/Validacao.API/api/Recurso";
            var url = baseUrl + validarToken + "/" + codigoProfissional;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<RecursoCCH>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<ProjetoRecursoCCH>> ProjetoRecurso(string codigoProfissional, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var validarToken = "/Validacao.API/api/Projeto/Recurso";
            var url = baseUrl + validarToken + "/" + codigoProfissional;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<List<ProjetoRecursoCCH>>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<ColaboradorCCH>> RecursoProjeto(string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var url = _baseURL + "/Validacao.API/api/Recurso/Gerentes";
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };
            var responseMessage = await _apiClient.GetAsync<List<ColaboradorCCH>>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<IEnumerable<ValidacaoProjetoResult>> ValidacaoProjeto(string codigoProjeto, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var url = _baseURL + "/Validacao.API/api/Projeto/" + codigoProjeto;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };
            var responseMessage = (await _apiClient.GetAsync<ValidacaoProjetoResult>(url, headers)).Resposta;

            var ret = new List<ValidacaoProjetoResult>();

            if (responseMessage != null)
            {
                ret.Add(responseMessage);
            }

            return ret;
        }

        public async Task<List<HierarquiaResult>> Hierarquia(string tokenUsuario)
        {
            var url = _baseURL_Hierarquia + "/Validacao.API/api/Recurso/Gerentes/Hierarquia";
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };
            var responseMessage = await _apiClient.GetAsync<List<HierarquiaResult>>(url, headers);

            if (responseMessage.Sucesso)
            {
                var ret = new List<HierarquiaResult>();
                foreach (HierarquiaResult resposta in responseMessage.Resposta)
                {
                    ret.Add(resposta);
                }
                return ret;
            }
            else
            {
                throw new Exception("Hierarquia vazia");
            }
        }

        public async Task<List<ColaboradorFourMakersCCH>> ColaboradoresFourMakers(string tokenUsuario)
        {
            var baseUrl = _baseURL_mapa_alocacao;
            var validarToken = "/scc.fourmakers/api/Colaborador";
            var url = baseUrl + validarToken;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<List<ColaboradorFourMakersCCH>>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<ColaboradorFourMakersCCH>> ColaboradorNomeFourMakers(string nmProfissional, string tokenUsuario)
        {
            var baseUrl = _baseURL_mapa_alocacao;
            var validarToken = "/scc.fourmakers/api/Colaborador/Nome";
            var url = baseUrl + validarToken + "/" + nmProfissional;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<List<ColaboradorFourMakersCCH>>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<ComboProjetosFourMakersCCH>> ComboProjetosFourMakers(string tokenUsuario)
        {
            var baseUrl = _baseURL_mapa_alocacao;
            var validarToken = "/scc.fourmakers/api/ComboProjetos";
            var url = baseUrl + validarToken;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<List<ComboProjetosFourMakersCCH>>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<ProjetoHorasFourMakersCCH>> ProjetoHorasFourMakers(string tokenUsuario)
        {
            var baseUrl = _baseURL_mapa_alocacao;
            var validarToken = "/scc.fourmakers/api/ProjetoHoras";
            var url = baseUrl + validarToken;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<List<ProjetoHorasFourMakersCCH>>(url, headers);

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