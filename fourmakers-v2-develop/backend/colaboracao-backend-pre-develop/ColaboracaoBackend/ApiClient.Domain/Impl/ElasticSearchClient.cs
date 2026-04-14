using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;

namespace ApiClient.Domain.Impl;

public class ElasticSearchClient(IApiClient apiClient) : IElasticSearchClient
{
    private readonly string _baseUrl =
        VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE) + VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.ELASTIC_SEARCH_API_PATH);
    
    public async Task SyncColaborador(string codigoInternoColaborador, string token)
    {
        var url = _baseUrl + "ElasticSearch/Sync/sync-colaborador";

        var headers = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>
            (
                "Authorization",
                String.Format("Bearer {0}", token)
            )
        };

        var responseMessage = await apiClient.PostAsync<dynamic>(new { codColaborador = codigoInternoColaborador }, url, headers);

        if (!responseMessage.Sucesso)
        {
            throw new Exception(responseMessage.Mensagem);
        }
    }
}