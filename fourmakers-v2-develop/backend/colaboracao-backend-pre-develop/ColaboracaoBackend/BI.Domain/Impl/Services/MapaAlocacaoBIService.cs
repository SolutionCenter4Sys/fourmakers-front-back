using ApiClient.Domain;
using ApiClient.Domain.Interfaces;
using BI.Domain.Interfaces.Services;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Usuario;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;
using DataTransferObject.Domain.Util.Enum;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace BI.Domain.Impl.Services;

[LogDomainClass]
public class MapaAlocacaoBIService : IMapaAlocacaoBIService
{
    private readonly ITokenSistemaRepository _tokenRepository;
    private readonly IMapaDeAlocacaoClient _mapaDeAlocacaoClient;
    private readonly ITokenSistemaService _tokenSistemaService;

    public MapaAlocacaoBIService(IMapaDeAlocacaoClient mapaDeAlocacaoClient,
                                 ITokenSistemaService tokenSistemaService)
    {
        _mapaDeAlocacaoClient = mapaDeAlocacaoClient;
        _tokenSistemaService = tokenSistemaService;
    }
    
    public async Task<ApiGenericResult<List<CalculoAderenciaComPerfilEColaboradorDTO>>> GetAderenciaAlocados(string tokenSistema)
    {
        var apiResult = new ApiGenericResult<List<CalculoAderenciaComPerfilEColaboradorDTO>>();
        try
        {
            var orgId = _tokenSistemaService.GetOrgTokenSistema(tokenSistema);
            
            if (orgId != (int)EnumORG.FOURSYS_2)
            {
                throw new AccessViolationException("Não autorizado para essa organização");
            }

            var tokenAcesso = VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO);
            apiResult.Retorno = (await _mapaDeAlocacaoClient.ListarAderenciaAlocados(tokenAcesso, orgId)).Retorno;
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Aderencia Alocacao");
        }
        return apiResult;
    }
}