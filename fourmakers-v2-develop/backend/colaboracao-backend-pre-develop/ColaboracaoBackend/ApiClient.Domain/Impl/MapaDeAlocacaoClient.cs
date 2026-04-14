using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;
using Microsoft.Extensions.Configuration;

namespace ApiClient.Domain.Impl;

public class MapaDeAlocacaoClient : IMapaDeAlocacaoClient
{
    private IApiClient _apiClient;
    private string _baseURL;

    public MapaDeAlocacaoClient(IApiClient apiClient, IConfiguration configuration)
    {
        _apiClient = apiClient;
        _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.COLABORACAO_BASE);
    }

    public async Task<ApiGenericResult<List<CalculoAderenciaComPerfilEColaboradorDTO>>> ListarAderenciaAlocados(string tokenSistema, int orgId)
    {
        var path = $"api/GestaoDeAlocados/Aderencia/ListarAderenciaAlocados?orgId={orgId}";
        var url = _baseURL + path;

        var headers = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>
            (
                "Authorization",
                String.Format("Bearer {0}", tokenSistema)
            )
        };

        var responseMessage = await _apiClient.GetAsync<ApiGenericResult<List<CalculoAderenciaComPerfilEColaboradorDTO>>>(url, headers);

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