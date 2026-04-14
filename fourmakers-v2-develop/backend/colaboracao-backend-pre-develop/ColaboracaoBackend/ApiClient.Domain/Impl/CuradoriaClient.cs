using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiClient.Domain.Interfaces;
using ApiClient.Infra.Interfaces;
using Colaboracao.Helper;
using Competencia.Domain.Enums;
using DataTransferObject.Domain.Curadoria;
using DataTransferObject.Domain.Match;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ApiClient.Domain.Impl;

public class CuradoriaClient(IApiClient apiClient, ILogger<UsuarioClient> logger) : ICuradoriaClient
{
    private readonly ILogger _log = logger;
    private readonly string _baseURL = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.CURADORIA_API);
    private readonly string _token = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_MATCH_API);

    public async Task<CuradoriaSkillResponse> ObterComparativoSkill(string request, TipoCompetenciaSRSEnum tipoCompetencia)
    {
        var encodingRequest = Uri.EscapeDataString(request);
        
        var url = _baseURL + ObterPathNameSkill(tipoCompetencia) + $"/?input={encodingRequest}";
        var headers = new List<KeyValuePair<string, string>>();
        headers.Add(new KeyValuePair<string, string> ( "X-API-Key", _token ));
        _log.LogInformation($"URL: {url}");
        var responseMessage = await apiClient.GetAsync<CuradoriaSkillResponse>(url, headers);
        
        if (responseMessage.Sucesso)
        {
            return responseMessage.Resposta;
        }
        else
        {
            _log.LogError(JsonConvert.SerializeObject("Erro na requisicao da Curadoria"));
            _log.LogError(responseMessage.Mensagem);
            throw new Exception(responseMessage.Mensagem);
        }
    }

    private string ObterPathNameSkill(TipoCompetenciaSRSEnum tipo)
    {
        return tipo switch
        {
            TipoCompetenciaSRSEnum.HardSkill => "/skills/hard-skill",
            TipoCompetenciaSRSEnum.SoftSkill => "/skills/soft-skill",
            TipoCompetenciaSRSEnum.Metodologia => "/skills/methodology-skill",
            TipoCompetenciaSRSEnum.Idioma => "/skills/language-skill",
            TipoCompetenciaSRSEnum.Dominio => "/skills/domain-skill",
            _ => throw new ArgumentOutOfRangeException(nameof(tipo), tipo, "Tipo de competência desconhecido")
        };
    }
}