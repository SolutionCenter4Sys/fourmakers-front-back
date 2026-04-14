using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Hobby;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class HobbyClient : IHobbyClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public HobbyClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.HOBBY_API_PATH);
        }

        public async Task<ItemPerfilDTO> GetHobbyById(long id, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var getHobbyById = _configuration["Clients:Hobby:GetHobbyById"];
            var url = baseUrl + getHobbyById + "?id=" + id;
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

        public async Task<List<ItemPerfilResult>> InserirHobbieColaborador(string token, List<AdicionarRemoverItemDTO> dtos)
        {
            var baseUrl = _baseURL;
            var inserirHobbie = _configuration["Clients:Hobby:AdicionarHobbieColaborador"];
            var url = baseUrl + inserirHobbie;
            var objeto = new { param = dtos };

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<HobbyColaboradorResult>(dtos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Respostas;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<HobbyColaboradorDTO>> ListarHobbiesColaborador(string cpf, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarHobbies = _configuration["Clients:Hobby:ListarHobbiesColaborador"];
            var url = baseUrl + listarHobbies + "?cpfColaborador=" + cpf;
            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaHobbyColaboradorResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Hobbies;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<StatusResult> MergeHobbieColaborador(MergeItemPerfilDTO dtos, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Hobby:MergeHobbieColaborador"];
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

        public async Task<StatusResult> RemoverHobbieColaborador(string token, string cpf, long id)
        {
            var baseUrl = _baseURL;
            var removerHobbie = _configuration["Clients:Hobby:RemoverHobbieColaborador"];
            var url = baseUrl + removerHobbie;
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
    }
}