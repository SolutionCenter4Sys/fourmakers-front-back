using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;
using Microsoft.Extensions.Configuration;

namespace ApiClient.Domain.Impl;

public class IAClient : IIAClient
{
    private IApiClient _apiClient;
    private IConfiguration _configuration;
    private string _baseURL;

    public IAClient(IApiClient apiClient, IConfiguration configuration)
    {
        _apiClient = apiClient;
        _configuration = configuration;
        _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CURRICULO_API_PATH);
    }
    
    public async Task<AnaliseDocumentoSolicitacaoDTO> AnalisarDocumentoIA(string base64Image,  string tokenAcesso)
    {
        var url = _baseURL + _configuration["Clients:Curriculo:ImageAnalysis"];

        var headers = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>
            (
                "Authorization",
                String.Format("Bearer {0}", tokenAcesso)
            )
        };
            
        var fileName = Guid.NewGuid() + ".png";

        var responseMessage = await _apiClient.PostAsync<AnaliseDocumentoSolicitacaoDTO>(new { fileName = fileName, contentType = "image/png", imageBase64 = base64Image }, url, headers);

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