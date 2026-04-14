using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Metodologia.MetodologiaClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class MetodologiaClient : IMetodologiaClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public MetodologiaClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COMPETENCIA_API_PATH);
        }

        public async Task<ItemPerfilResult> AlterarMetodologiaColaborador(AdicionarRemoverItemDTO dtos, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Metodologia:AlterarMetodologiaColaborador"];
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

        public async Task<ItemPerfilDTO> GetMetodologiaById(long id, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var getMetodologiaById = _configuration["Clients:Metodologia:GetMetodologiaById"];
            var url = baseUrl + getMetodologiaById + "?id=" + id;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<GetItemPerfilResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.ItemPerfil;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        //TODO: Gabrielle - esse método precisa ser revisado pois não passa a descricao
        public async Task<List<ItemPerfilResult>> InserirMetodologiaColaborador(string token, List<AdicionarRemoverItemDTO> dtos)
        {
            var baseUrl = _baseURL;
            var inserirMetodologia = _configuration["Clients:Metodologia:AdicionarMetodologiaColaborador"];
            var url = baseUrl + inserirMetodologia;

            var objeto = new { param = dtos };

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<MetodologiaColaboradorResult>(dtos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Respostas;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<MetodologiaColaboradorDTO>> ListarMetodologiasColaborador(string cpf, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarMetodologias = _configuration["Clients:Metodologia:ListarMetodologiasColaborador"];
            var url = baseUrl + listarMetodologias + "?cpfColaborador=" + cpf;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaMetodologiaColaboradorResultClient>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Metodologias;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }
        //TODO: Gabrielle - Método não utilizado/ em confirmação com o Pedro para deletar
        public async Task<StatusResult> MergeMetodologiaColaborador(MergeItemPerfilDTO dtos, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Metodologia:MergeMetodologiaColaborador"];
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
        //TODO: Gabrielle - ainda não implementado na controller atual
        public async Task<StatusResult> RemoverMetodologiaColaborador(string token, string cpf, long id)
        {
            var baseUrl = _baseURL;
            var removerMetodologia = _configuration["Clients:Metodologia:RemoverMetodologiaColaborador"];
            var url = baseUrl + removerMetodologia;
            var objeto = new
            {
                Id = id,
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

        public async Task<List<KeyValuePair<string, long>>> GetMetodologiaInfoByDescricao(List<string> metodologias, string token)
        {
            var baseUrl = _baseURL;
            var endpoint = _configuration["Clients:Metodologia:GetMetodologiaInfoByDescricao"];
            var url = baseUrl + endpoint;

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<List<KeyValuePair<string, long>>>(new { Metodologias = metodologias }, url, headers);

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