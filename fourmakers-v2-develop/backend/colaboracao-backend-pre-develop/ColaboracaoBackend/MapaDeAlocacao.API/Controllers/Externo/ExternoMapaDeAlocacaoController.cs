using Colaboracao.Core;
using Colaboracao.Helper.Util;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.Externo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MapaDeAlocacao.API.Controllers.Externo;

[Route("api/MapaDeAlocacao/Externo")]
[HandleException]
[ApiController]
[LogAction]
public class ExternoMapaDeAlocacaoController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapaDeAlocacaoExternoService _mapaDeAlocacaoExternoService;

    public ExternoMapaDeAlocacaoController(
        IHttpContextAccessor httpContextAccessor,
        IMapaDeAlocacaoExternoService mapaDeAlocacaoExternoService)
    {
        _httpContextAccessor = httpContextAccessor;
        _mapaDeAlocacaoExternoService = mapaDeAlocacaoExternoService;
    }

    [HttpGet("ObterAlocacoesHorasMensais")]
    public async Task<IActionResult> ObterAlocacoesHorasMensais(
        int mes,
        int ano,
        string codigoColaborador = null,
        string codigoProjeto = null)
    {
        var tokenSistema = AuthorizationUtil.ObterTokenBearer(_httpContextAccessor.HttpContext);

        var retorno = await _mapaDeAlocacaoExternoService.ObterAlocacoesHorasMensais(
            tokenSistema,
            mes,
            ano,
            codigoColaborador,
            codigoProjeto);

        return Ok(retorno);
    }
}
