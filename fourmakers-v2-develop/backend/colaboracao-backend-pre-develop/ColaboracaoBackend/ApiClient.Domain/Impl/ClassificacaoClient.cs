using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Classificacao.Candidato;
using DataTransferObject.Domain.Classificacao.Perfil;
using Microsoft.Extensions.Configuration;

namespace ApiClient.Domain.Impl;

public class ClassificacaoClient(IApiClient apiClient, IConfiguration configuration) : IClassificacaoClient
{
    private readonly string _baseUrl = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CURRICULO_API_PATH);
    private readonly string _token = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_CLASSIFICATION_API);
    
    public async Task<CandidatoClassificacaoResult> ClassificarCandidatosAsync(List<CandidatoClassificacaoDTO> candidatoClassificacaoDTOs)
    {
        var url = _baseUrl + "ClassificadorML/classificar";

        var headers = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>
            (
                "Authorization",
                String.Format("Bearer {0}", _token)
            )
        };
            

        var responseMessage = await apiClient.PostAsync<CandidatoClassificacaoResult>(new { candidatos = candidatoClassificacaoDTOs }, url, headers);

        if (responseMessage.Sucesso)
        {
            return responseMessage.Resposta;
        }
        else
        {
            throw new Exception(responseMessage.Mensagem);
        }
    }

    public async Task<PerfilClassificacaoResult> ClassificarPerfisAsync(List<PerfilClassificacaoDTO> perfilClassificacaoDTOs)
    {
        var url = _baseUrl + "ClassificadorML/classificar-vagas";

        var headers = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>
            (
                "Authorization",
                String.Format("Bearer {0}", _token)
            )
        };

        var responseMessage = await apiClient.PostAsync<PerfilClassificacaoResult>(new { vagas = perfilClassificacaoDTOs }, url, headers);

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