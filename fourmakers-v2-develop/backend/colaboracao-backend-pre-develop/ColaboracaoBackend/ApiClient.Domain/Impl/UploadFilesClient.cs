using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiClient.Domain.Impl
{
    public class UploadFilesClient : IUploadFilesClient
    {
        private IApiClient _apiClient;
        private IConfiguration _configuration;
        private string _baseURL;

        public UploadFilesClient(IApiClient apiClient, IConfiguration configuration)
        {
            _apiClient = apiClient;
            _configuration = configuration;
            _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.UPLOADFILES_API_PATH);
        }

        public async Task<StatusResult> DeleteFile(string keyName)
        {
            var baseUrl = _baseURL;
            var validarToken = _configuration["Clients:UploadFile:DeleteFileUrl"];
            var url = baseUrl + validarToken;

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO))
                )
            };
            var objeto = new
            {
                keyName = keyName
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

        public async Task<StatusResult> UploadFile(string fileName, byte[] arquivo)
        {
            var baseUrl = _baseURL;

            var uploadFileUrl = _configuration["Clients:UploadFile:UploadFileUrl"];
            var url = baseUrl + uploadFileUrl;

            var headers = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>
                (
                    "Authorization",
                    String.Format("Bearer {0}", VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO))
                )
            };

            MultipartFormDataContent form = new MultipartFormDataContent();

            form.Add(new ByteArrayContent(arquivo, 0, arquivo.Length), "arquivo", fileName);

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

        
    }
}