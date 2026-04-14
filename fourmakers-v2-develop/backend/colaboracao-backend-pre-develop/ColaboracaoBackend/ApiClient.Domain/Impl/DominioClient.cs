using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Dominio;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class DominioClient : IDominioClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public DominioClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COMPETENCIA_API_PATH);
        }

        public async Task<ItemPerfilResult> AlterarDominioColaborador(AdicionarRemoverItemDTO dtos, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Dominio:AlterarDominioColaborador"];
            var url = baseUrl + listarCompetencias;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.PostAsync<ItemPerfilResult>(dtos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<ItemPerfilDTO> GetDominioById(long id, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var getDominioById = _configuration["Clients:Dominio:GetDominioById"];
            var url = baseUrl + getDominioById + $"?id={id}";

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<DominioResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Dominio;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<ItemPerfilResult>> InserirDominioColaborador(string token, List<AdicionarRemoverItemDTO> dtos)
        {
            var baseUrl = _baseURL;
            var inserirDominio = _configuration["Clients:Dominio:AdicionarDominioColaborador"];
            var url = baseUrl + inserirDominio;
            var objeto = new { param = dtos };

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<DominioColaboradorResult>(dtos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Respostas;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<DominioColaboradorDTO>> ListarDominiosColaborador(string cpf, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarDominios = _configuration["Clients:Dominio:ListarDominioColaborador"];
            var url = baseUrl + listarDominios + "?cpfColaborador=" + cpf;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaDominioColaboradorResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Dominio;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<StatusResult> MergeDominioColaborador(MergeItemPerfilDTO dtos, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Dominio:MergeDominioColaborador"];
            var url = baseUrl + listarCompetencias;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.PostAsync<StatusResult>(dtos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<StatusResult> RemoverDominioColaborador(string token, string cpf, long id)
        {
            var baseUrl = _baseURL;
            var removerDominio = _configuration["Clients:Dominio:RemoverDominioColaborador"];
            var url = baseUrl + removerDominio;
            var objeto = new
            {
                id = id,
                Cpf = cpf
            };

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

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

        public async Task<List<KeyValuePair<string, long>>> GetDominioInfoByDescricao(List<string> dominios, string token)
        {
            var baseUrl = _baseURL;
            var endpoint = _configuration["Clients:Dominio:GetDominioInfoByDescricao"];
            var url = baseUrl + endpoint;

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<List<KeyValuePair<string, long>>>(new { Dominios = dominios }, url, headers);

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