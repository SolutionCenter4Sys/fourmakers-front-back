using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Formacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class FormacaoClient : IFormacaoClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public FormacaoClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.FORMACAO_API_PATH);
        }

        public async Task<ItemPerfilResult> AlterarFormacaoColaborador(AdicionarRemoverItemDTO dtos, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Formacao:AlterarFormacaoColaborador"];
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

        public async Task<FormacaoDTO> GetFormacaoById(long id, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var getFormacaoById = _configuration["Clients:Formacao:GetFormacaoById"];

            var url = baseUrl + getFormacaoById + $"?id={id}";

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<FormacaoResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Formacao;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<CertificadoDTO> InserirCertificadoFormacaColaborador(string cpf, long formacaoColaboradorId, byte[] file, TipoCertificadoEnum tipo, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var inserirCertificadoFormacao = _configuration["Clients:Formacao:InserirCertificadoFormacaColaborador"];
            var url = baseUrl + inserirCertificadoFormacao;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new StringContent(cpf), "cpf");
            form.Add(new StringContent(formacaoColaboradorId.ToString()), "formacaoColaboradorId");
            form.Add(new StringContent(tipo.ToString()), "tipo");
            form.Add(new ByteArrayContent(file, 0, file.Length), "arquivo", "arquivo");
            var responseMessage = await _apiClient.PostMultiFormAsync<CertificadoResult>(form, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Certificado;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<ItemPerfilResult>> InserirFormacaoColaborador(string token, List<AdicionarRemoverItemDTO> dtos)
        {
            var baseUrl = _baseURL;
            var inserirFormacao = _configuration["Clients:Formacao:AdicionarFormacaoColaborador"];
            var url = baseUrl + inserirFormacao;
            var objeto = new { param = dtos };

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",
                    String.Format("Bearer {0}", token))
            };

            var responseMessage = await _apiClient.PostAsync<FormacaoColaboradorResult>(dtos, url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Respostas;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<List<FormacaoColaboradorDTO>> ListarFormacoesColaborador(string cpf, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarFormacoes = _configuration["Clients:Formacao:ListarFormacaoColaborador"];
            var url = baseUrl + listarFormacoes + "?cpfColaborador=" + cpf;

            var headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>
                    (
                        "Authorization",
                        String.Format("Bearer {0}", tokenUsuario)
                    )
                };

            var responseMessage = await _apiClient.GetAsync<ListaFormacaoColaboradorResult>(url, headers);

            if (responseMessage.Sucesso)
            {
                return responseMessage.Resposta.Formacao;
            }
            else
            {
                throw new Exception(responseMessage.Mensagem);
            }
        }

        public async Task<StatusResult> MergeFormacaoColaborador(MergeItemPerfilDTO dtos, string tokenUsuario)
        {
            var baseUrl = _baseURL;
            var listarCompetencias = _configuration["Clients:Formacao:MergeFormacaoColaborador"];
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

        public async Task<StatusResult> RemoverFormacaoColaborador(string token, string cpf, long id)
        {
            var baseUrl = _baseURL;
            var removerFormacao = _configuration["Clients:Formacao:RemoverFormacaoColaborador"];
            var url = baseUrl + removerFormacao;
            var objeto = new
            {
                FormacaoId = id,
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