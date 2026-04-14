using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Cbo;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.API.Controllers;

[Authorize]
[Route("api/Admissao/[controller]")]
[ApiController]
[LogAction]
public class CboController : ControllerBase
{
    private readonly ICboService _cboService;
    private readonly IAspNetUser _aspNetUser;

    public CboController(ICboService cboService, IAspNetUser aspNetUser)
    {
        _cboService = cboService;
        _aspNetUser = aspNetUser;
    }

    /// <summary>
    /// Lista CBOs (Código Brasileiro de Ocupações). Requer funcionalidade ADMISSAO_CBO_LISTAR.
    /// </summary>
    [HttpGet("Listar")]
    public async Task<ActionResult<ApiGenericResult<List<CboResult>>>> Listar(
        [FromQuery] bool somenteAtivos = false,
        [FromQuery] int? cursor = null,
        [FromQuery] int? limite = null,
        [FromQuery] string busca = null)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _cboService.ListarAsync(usuario.Cpf, usuario.OrgId, somenteAtivos, cursor, limite, busca);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um CBO pelo ID. Requer funcionalidade ADMISSAO_CBO_LISTAR.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<CboResult>>> ObterPorId(Guid id)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _cboService.ObterPorIdAsync(usuario.Cpf, usuario.OrgId, id);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Obtém um CBO pelo código (ex: 2124-05). Requer funcionalidade ADMISSAO_CBO_LISTAR.
    /// </summary>
    [HttpGet("PorCodigo/{codigo}")]
    public async Task<ActionResult<ApiGenericResult<CboResult>>> ObterPorCodigo(string codigo)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _cboService.ObterPorCodigoAsync(usuario.Cpf, usuario.OrgId, codigo);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Cria um novo CBO. Requer funcionalidade ADMISSAO_CBO_CRIAR_EDITAR.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiGenericResult<CboResult>>> Inserir([FromBody] CboInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _cboService.InserirAsync(usuario.Cpf, usuario.OrgId, input);
        if (!result.Sucesso)
            return BadRequest(result);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Retorno?.Id }, result);
    }

    /// <summary>
    /// Atualiza um CBO existente. Requer funcionalidade ADMISSAO_CBO_CRIAR_EDITAR.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<CboResult>>> Atualizar(Guid id, [FromBody] CboInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _cboService.AtualizarAsync(usuario.Cpf, usuario.OrgId, id, input);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        if (!result.Sucesso)
            return BadRequest(result);
        return Ok(result);
    }
}
