using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Util;
using Core.Domain.Reembolso.Solicitacao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Reembolso.Solicitacao;
using Financeiro.Domain.Services.Reembolso.Solicitacao;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.Reembolso;

[Route("api/Financeiro/Reembolso/Externo")]
[HandleException]
[ApiController]
[LogAction]
public class ExternoReembolsoController: ControllerBase
{

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISolicitacaoExternoService _solicitacaoExternoService;

    public ExternoReembolsoController(IHttpContextAccessor httpContextAccessor,
                             ISolicitacaoExternoService solicitacaoExternoService)
    {
        _httpContextAccessor = httpContextAccessor;
        _solicitacaoExternoService = solicitacaoExternoService;
    }

    [HttpGet("ProcessarReembolso")]
    public async Task<IActionResult> ProcessarReembolso(string cnpj = "", bool atualizarStatusParaPago = false, string codigoColaboradorExternoAprovador = "")
    {
        var tokenSistema = AuthorizationUtil.ObterTokenBearer(_httpContextAccessor.HttpContext);

        var retorno = await _solicitacaoExternoService.ProcessarReembolsoAsync(tokenSistema, atualizarStatusParaPago, cnpj, codigoColaboradorExternoAprovador);

        return Ok(retorno);

    }


}