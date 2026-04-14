using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.BotFourmakers;
using DataTransferObject.Domain.BotFourmakers.ChatIA;
using DataTransferObject.Domain.VagasSRS;

namespace ApiClient.Domain.Impl;

public class ChatIAClient : IChatIAClient
{
    private IApiClient _apiClient;
    private string _baseURL;
    private string _apiKey;
    private string _questionPath;
    private string _feedbackPath;

    public ChatIAClient(IApiClient apiClient)
    {
        _apiClient = apiClient;
        _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BASE_URL_BOT_FOURMAKERS);
        _questionPath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BOT_FOURMAKERS_QUESTION);
        _feedbackPath = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BOT_FOURMAKERS_FEEDBACK);
        _apiKey = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.BOT_FOURMAKERS_API_KEY);
    }

    public async Task<ChatIAQuestionReponseDTO> PostQuestionAsync(string question, int orgId)
    {
        var baseUrl = _baseURL;
        var url = baseUrl + _questionPath;
        
        var objeto = new { query = question, orgid = orgId };
        
        var headers = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>
            (
                "X-API-KEY",
               _apiKey
            )
        };
        
        var responseMessage = await _apiClient.PostAsync<ChatIAQuestionReponseDTO>(objeto, url, headers);

        if (responseMessage.Sucesso)
        {
            return responseMessage.Resposta;
        }
        else
        {
            throw new Exception(responseMessage.Mensagem);
        }
    }
    
    public async Task<string> PostFeedbackAsync(string question, string response, string query, string queryHabilidades, bool feedback)
    {
        var baseUrl = _baseURL;
        var url = baseUrl + _feedbackPath;
        
        var objeto = new { question, response, query_mapa_alocacao = query, query_habilidade = queryHabilidades, feedback };
        
        var headers = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>
            (
                "X-API-KEY",
                _apiKey
            )
        };
        
        var responseMessage = await _apiClient.PostAsync<string>(objeto, url, headers);

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