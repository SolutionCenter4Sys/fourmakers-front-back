using Colaboracao.Core;
using Colaboracao.Helper.Util;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.Externo;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.Externo;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Financeiro.API.Controllers.IntegracaoBancaria;

[Route("api/Financeiro/IntegracaoBancaria/Externo")]
[HandleException]
[ApiController]
public class ExternoIntegracaoBancariaController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIntegracaoBancariaExternaService _integracaoBancariaExternaService;

    public ExternoIntegracaoBancariaController(
        IHttpContextAccessor httpContextAccessor,
        IIntegracaoBancariaExternaService integracaoBancariaExternaService)
    {
        _httpContextAccessor = httpContextAccessor;
        _integracaoBancariaExternaService = integracaoBancariaExternaService;
    }

    [HttpGet("BuscarReembolsosPagosViaCNAB")]
    public async Task<IActionResult> BuscarReembolsosPagosViaCNAB()
    {
        var tokenSistema = AuthorizationUtil.ObterTokenBearer(_httpContextAccessor.HttpContext);

        var retorno = await _integracaoBancariaExternaService.BuscarReembolsosPagosViaCNAB(tokenSistema);

        return Ok(retorno);
    }

    /// <summary>
    /// Lista holerites (líquido no JSON de retorno do item de lote) com conciliação de folha ponto aprovada.
    /// Org vem do token de sistema (Bearer). Filtros: mês, ano e opcionalmente código de diretoria.
    /// </summary>
    [HttpGet("ListarHoleritesLiquidosConciliacaoFolhaPonto")]
    public async Task<ActionResult<ApiGenericResult<List<HoleriteLiquidoConciliadoExternoDTO>>>> ListarHoleritesLiquidosConciliacaoFolhaPonto(
        [FromQuery] int mes,
        [FromQuery] int ano,
        [FromQuery] string codDiretoria = null)
    {
        var tokenSistema = AuthorizationUtil.ObterTokenBearer(_httpContextAccessor.HttpContext);

        var retorno = await _integracaoBancariaExternaService.ListarHoleritesLiquidosConciliacaoFolhaPontoAsync(
            tokenSistema,
            mes,
            ano,
            codDiretoria);

        if (!retorno.Sucesso)
            return BadRequest(retorno);

        return Ok(retorno);
    }
}
