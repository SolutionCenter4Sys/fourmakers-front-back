using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Reembolso.Verba;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.Reembolso;

[Authorize]
[Route("api/Financeiro/Reembolso/[controller]")]
[HandleException]
[LogAction]
public class VerbaController: ControllerBase
{
    private readonly IVerbaLogService _verbaLogService;
    private readonly IVerbaService _verbaService;
    private readonly IVerbaTipoService _verbaTipoService;
    private UsuarioLogadoDTO _usuarioLogado;

    public VerbaController(IVerbaLogService verbaLogService, IAspNetUser aspNetUser, IVerbaService verbaService, IVerbaTipoService verbaTipoService)
    {
        _verbaLogService = verbaLogService;
        _verbaService = verbaService;
        _verbaTipoService = verbaTipoService;
        _usuarioLogado = aspNetUser.GetUsuarioLogado();
    }

    [HttpGet("ListarVerbaLogs")]
    public async Task<ActionResult<ApiGenericResult<List<VerbaLogDTO>>>> ListarVerbaLogs()
    {
        return Ok(await _verbaLogService.ListarLogsAsync(_usuarioLogado.OrgId));
    }
    
    [HttpPost("EditarVerba")]
    public async Task<ActionResult<ApiGenericResult<VerbaEParametroDTO>>> EditarVerba([FromBody] VerbaEParametroDTO input)
    {
        return Ok(await _verbaService.EditarAsync(input, _usuarioLogado.OrgId, _usuarioLogado.Cpf));
    }
    
    [HttpGet("ObterVerbasEParametro")]
    public async Task<ActionResult<ApiGenericResult<VerbaEParametroDTO>>> ObterVerbasEParametro()
    {
        return Ok(await _verbaService.ObterVerbasEParametro(_usuarioLogado.OrgId, _usuarioLogado.Cpf));
    }

    [HttpGet("ListarVerbaTipo")]
    public async Task<ActionResult<ApiGenericResult<VerbaTipoDTO>>> ListarVerbaTipo()
    {
        return Ok(await _verbaTipoService.ListarAsync(_usuarioLogado.OrgId));
    }
    
    [HttpGet("ListarVerbas")]
    public async Task<ActionResult<ApiGenericResult<VerbaTipoDTO>>> ListarVerbas()
    {
        return Ok(await _verbaService.ListarSimplificadoAsync(_usuarioLogado.OrgId));
    }
    
    [HttpGet("ListarVerbasComExecao")]
    public async Task<ActionResult<ApiGenericResult<VerbaTipoDTO>>> ListarVerbasComExcecao([FromQuery] string codigoProjeto, string codigoCliente)
    {
        return Ok(await _verbaService.ListarSimplificadoComExcecaoAsync(_usuarioLogado.OrgId, _usuarioLogado.Cpf, codigoProjeto, codigoCliente));
    }
    
    [HttpGet("ObterVerbasEParametroValidacao")]
    public async Task<ActionResult<ApiGenericResult<VerbaEParametroValidacaoDTO>>> ObterVerbasEParametroValidacao()
    {
        return Ok(await _verbaService.ObterVerbasEParametroValidacao(_usuarioLogado.OrgId, _usuarioLogado.Cpf));
    }
}