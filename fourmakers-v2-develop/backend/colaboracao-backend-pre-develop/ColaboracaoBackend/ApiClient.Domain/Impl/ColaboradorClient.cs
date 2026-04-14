using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.MapaDeAlocacao.CargaMapaAlocacao;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class ColaboradorClient : IColaboradorClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private ILogger _log;
        private string _baseURL;
        private string _apiCargaMapaAlocacao;

        public ColaboradorClient(IApiClient apiClient, IConfiguration configuration, ILogger<ColaboradorClient> logger)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE)
                + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORADOR_API_PATH);
            _apiCargaMapaAlocacao = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORADOR_API_CARGA_MAPA);
            _log = logger;
        }

        public async Task<bool> AlterarDadosColaborador(ColaboradorDTO colabInfo, byte[] imagem, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var alterarDados = _configuration["Clients:Colaborador:AlterarDadosColaborador"];
            var url = baseUrl + alterarDados;

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent(JsonSerializer.Serialize(colabInfo)), "colaborador");
            form.Add(new ByteArrayContent(imagem, 0, imagem.Length), "imagem", "imagem");

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.PostMultiFormAsync<StatusResult>(form, url, headers);

            if (responseMessage.Sucesso)
            {
                return true;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<ListaCandidatoResult> BuscarCandidato(string cpf, string busca, int cursor, int limite, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            string buscarCandidato = _configuration["Clients:Colaborador:BuscarCandidato"];
            var url = baseUrl + buscarCandidato + "?cpf=" + cpf + "&busca=" + busca + "&cursor=" + cursor + "&limite=" + limite;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaCandidatoResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<ColaboradorDTO> BuscarDadosColaboradorAdmin(string cpf, string tokenUsuario)
        {
            tokenUsuario = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO);

            return await GetColaboradorByCpf(cpf, tokenUsuario);
        }

        public async System.Threading.Tasks.Task<ColaboradorDTO> GetColaboradorByCpf(string cpf, string tokenUsuario, bool lancarExcpNaoAutorizado = false)
        {
            var baseUrl = _baseURL;
            string validarToken = _configuration["Clients:Colaborador:BuscarDadosColaboradorSimple"];

            var url = baseUrl + validarToken + "?cpf=" + cpf;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<SimpleColaboradorResult>(url, headers);

            if (lancarExcpNaoAutorizado && responseMessage.Resposta.Colaborador is null)
            {
                if (!string.IsNullOrEmpty(responseMessage.Resposta.Mensagem) && responseMessage.Resposta.Mensagem.ToLower() == "acesso negado")
                {
                    throw new UnauthorizedAccessException("Erro ao recuperar dados do colaborador. Acesso negado.");
                }
            }

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Colaborador;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem + " for " + cpf);
            }
        }

        public async Task<StatusResult> InsereColaboradorCandidato(ColaboradorDTO colabInfo, byte[] imagem)
        {
            var baseUrl = _baseURL;
            var insereColaborador = _configuration["Clients:Colaborador:InsereColaboradorCandidato"];
            var url = baseUrl + insereColaborador;

            var headers = new List<KeyValuePair<string, string>>();

            MultipartFormDataContent form = new MultipartFormDataContent();
            form.Add(new StringContent(JsonSerializer.Serialize(colabInfo)), "colaborador");

            if (imagem != null)
            {
                form.Add(new ByteArrayContent(imagem, 0, imagem.Length), "arquivo", "imagem");
            }

            var responseMessage = await _apiClient.PostMultiFormAsync<StatusResult>(form, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<RedeColaboradorDTO> BuscarRedeColaborador(string cpf, string token)
        {
            var baseUrl = _baseURL;
            string buscarRedeColaborador = _configuration["Clients:Colaborador:BuscarRedeColaborador"];
            var url = baseUrl + buscarRedeColaborador + "?cpf=" + cpf;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", token)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<RedeColaboradorResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.RedeColaborador;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<SimpleColaboradorDTO> BuscarNomeColaborador(string cpf, string token)
        {
            var baseUrl = _baseURL;
            string buscarNomeColaborador = _configuration["Clients:Colaborador:BuscarNomeColaborador"];
            var url = baseUrl + buscarNomeColaborador + "?cpfColaborador=" + cpf;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", token)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<NomeColaboradorResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Colaborador;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<StatusResult> EnviaPushNotificationRedeColaborador(string cpf, string titulo, string mensagem, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var insereColaborador = _configuration["Clients:Colaborador:EnviaPushNotificationRedeColaborador"];
            var url = baseUrl + insereColaborador;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var objeto = new { cpf = cpf, titulo = titulo, mensagem = mensagem };

            var responseMessage = await _apiClient.PostAsync<StatusResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<ColaboradorDTO>> BuscarColaborador(string cpf, string busca, int cursor, int limite, string token)
        {
            var baseUrl = _baseURL;
            string buscarColaborador = _configuration["Clients:Colaborador:BuscarColaborador"];
            var url = baseUrl + buscarColaborador + "?cpf=" + cpf + "&busca=" + busca + "&cursor=" + cursor + "&limite=" + limite;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", token)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<BuscaColaboradorResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Colaboradores;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<CargaMapaAlocacaoResult> BuscarCargaMapaAlocacao(int cursor, int limit)
        {
            var urlBubble = _apiCargaMapaAlocacao + "?cursor=" + cursor + "&limit=" + limit;

            var headers = new List<KeyValuePair<string, string>>
            {
                //new KeyValuePair<string, string>
                //(
                //    "content-type","application/json"
                //)
            };

            var responseMessage = await _apiClient.GetAsync<CargaMapaAlocacaoResult>(urlBubble, headers);

            if (responseMessage.HttpStatus == HttpStatusCode.Unauthorized.ToString())
            {
                throw new UnauthorizedAccessException("Erro ao recuperar carga do mapa de alocação no Bubble API: Acesso negado.");
            }

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<bool> SincronizarPerfilLinkedinServicoExterno(string codigoInternoColaborador, string profileUrl, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var sincronizarPerfil = _configuration["Clients:CurriculoColaborador:SincronizarPerfilLinkedinServicoExterno"];
            var url = baseUrl + sincronizarPerfil;

            url += $"?codigoInternoColaborador={codigoInternoColaborador}";

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var objeto = new { profileUrl };

            var responseMessage = await _apiClient.PostAsync<StatusResult>(objeto, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Sucesso;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
    }
}