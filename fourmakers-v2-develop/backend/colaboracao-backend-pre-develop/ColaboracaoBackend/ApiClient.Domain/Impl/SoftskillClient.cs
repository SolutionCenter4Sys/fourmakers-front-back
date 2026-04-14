using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Softskill;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class SoftskillClient : ISoftskillClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public SoftskillClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = (VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COMPETENCIA_API_PATH));
        }

        public async Task<ItemPerfilResult> AlterarSoftskillColaborador(AdicionarRemoverItemDTO dtos, string tokenUsuario)
        {
            //var baseUrl = "http://localhost:12024/api/";
            var baseUrl = _baseURL;
            var listarSoftskill = _configuration["Clients:Softskill:AlterarSoftskillColaborador"];
            var url = baseUrl + listarSoftskill;

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

        public async Task<ItemPerfilDTO> GetSoftskillById(long id, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var getSoftskillById = _configuration["Clients:Softskill:GetSoftskillById"];
            var url = baseUrl + getSoftskillById + $"?id={id}";

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<SoftskillResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Softskill;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<ItemPerfilResult>> InserirSoftskillColaborador(string token, List<AdicionarRemoverItemDTO> dtos)
        {
            var baseUrl = _baseURL;
            //var baseUrl = "http://localhost:12024/api/";
            var inserirSoftskill = _configuration["Clients:Softskill:AdicionarSoftskillColaborador"];
            var url = baseUrl + inserirSoftskill;
            var objeto = new { param = dtos };

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<SoftskillColaboradorResult>(dtos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Respostas;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<SoftskillColaboradorDTO>> ListarSoftskillsColaborador(string cpf, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            //var baseUrl = "http://localhost:12024/api/";
            var listarSoftskills = _configuration["Clients:Softskill:ListarSoftskillColaborador"];
            var url = baseUrl + listarSoftskills + "?cpfColaborador=" + cpf;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaSoftskillColaboradorResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Softskill;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<StatusResult> MergeSoftskillColaborador(MergeItemPerfilDTO dtos, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            //var baseUrl = "http://localhost:12024/api/";
            var listarSoftskills = _configuration["Clients:Softskill:MergeSoftskillColaborador"];
            var url = baseUrl + listarSoftskills;

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

        public async Task<StatusResult> RemoverSoftskillColaborador(string token, string cpf, long id)
        {
            var baseUrl = _baseURL;
            //var baseUrl = "http://localhost:12024/api/";
            var removerSoftskill = _configuration["Clients:Softskill:RemoverSoftskillColaborador"];
            var url = baseUrl + removerSoftskill;
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

        public async Task<List<KeyValuePair<string, long>>> GetSoftSkillInfoByDescricao(List<string> softSkills, string token)
        {
            var baseUrl = _baseURL;
            var endpoint = _configuration["Clients:Softskill:GetSoftSkillInfoByDescricao"];
            var url = baseUrl + endpoint;

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<List<KeyValuePair<string, long>>>(new { SoftSkills = softSkills }, url, headers);

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