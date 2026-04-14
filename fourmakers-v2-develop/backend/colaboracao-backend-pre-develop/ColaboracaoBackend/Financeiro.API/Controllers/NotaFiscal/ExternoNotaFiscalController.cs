using Colaboracao.Core;
using Colaboracao.Helper.Util;
using DataTransferObject.Domain.Financeiro.NotaFiscal;
using Financeiro.Domain.Interfaces.NotaFiscal;
using Microsoft.AspNetCore.Mvc;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.NotaFiscal;

[Route("api/Financeiro/NotaFiscal/Externo")]
[HandleException]
[ApiController]
[LogAction]
public class ExternoNotaFiscalController : ControllerBase
{

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly INotaFiscalExternoService _notaFiscalExternoService;

    public ExternoNotaFiscalController(IHttpContextAccessor httpContextAccessor,
                             INotaFiscalExternoService notaFiscalExternoService)
    {
        _httpContextAccessor = httpContextAccessor;
        _notaFiscalExternoService = notaFiscalExternoService;
    }

    [HttpGet("ProcessarNotaFiscalAprovada")]
    public async Task<IActionResult> ProcessarNotaFiscalAprovada(string competencia, string? codDiretoria, string? documentoColaborador, bool atualizarStatusParaPago = false, string codigoColaboradorExternoAprovador = "", string numeroNf = "")
    {
        var tokenSistema = AuthorizationUtil.ObterTokenBearer(_httpContextAccessor.HttpContext);

        var retorno = await _notaFiscalExternoService.ProcessarNotaFiscalAprovadaAsync(tokenSistema, competencia, atualizarStatusParaPago, codDiretoria, documentoColaborador, codigoColaboradorExternoAprovador, numeroNf);

        return Ok(retorno);

    }
    
    [HttpGet("LiberarEmissaoDeNotasFiscaisPorVigencia")]
    public async Task<IActionResult> LiberarEmissaoDeNotasFiscaisPorVigencia(string competencia, string? codDiretoria, bool enviarEmail, string codigoColaboradorExternoEmissao)
    {
        var tokenSistema = AuthorizationUtil.ObterTokenBearer(_httpContextAccessor.HttpContext);

        var result = await _notaFiscalExternoService.LiberarEmissaoDeNotasFiscaisPorVigenciaExterno(tokenSistema, competencia, codDiretoria, enviarEmail, codigoColaboradorExternoEmissao);

        return Ok(result);

    }

}