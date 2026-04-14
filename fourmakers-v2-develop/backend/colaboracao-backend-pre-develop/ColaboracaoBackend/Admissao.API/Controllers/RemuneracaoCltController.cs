using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.RemuneracaoClt;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.API.Controllers;

[Authorize]
[Route("api/Admissao/[controller]")]
[ApiController]
[LogAction]
public class RemuneracaoCltController : ControllerBase
{
    private readonly IRemuneracaoCltService _service;
    private readonly IAspNetUser _aspNetUser;

    public RemuneracaoCltController(IRemuneracaoCltService service, IAspNetUser aspNetUser)
    {
        _service = service;
        _aspNetUser = aspNetUser;
    }

    /// <summary>
    /// Lista Remunerações CLT (cargo de admissão GUID, faixas, CBO e piso). Requer ADMISSAO_REMUNERACAO_CLT_LISTAR.
    /// </summary>
    [HttpGet("Listar")]
    public async Task<ActionResult<ApiGenericResult<List<RemuneracaoCltResult>>>> Listar(
        [FromQuery] bool somenteAtivos = true,
        [FromQuery] int? cursor = null,
        [FromQuery] int? limite = null,
        [FromQuery] Guid? admissaoCargoId = null)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ListarAsync(usuario.Cpf, usuario.OrgId, somenteAtivos, cursor, limite, admissaoCargoId);
        return Ok(result);
    }

    /// <summary>
    /// Obtém Remuneração CLT pelo ID. Requer ADMISSAO_REMUNERACAO_CLT_LISTAR.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<RemuneracaoCltResult>>> ObterPorId(Guid id)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ObterPorIdAsync(usuario.Cpf, usuario.OrgId, id);
        if (!result.Sucesso && result.Retorno == null) return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Obtém Remuneração CLT pelo ID do cargo de admissão (tb_admissao_cargo). Requer ADMISSAO_REMUNERACAO_CLT_LISTAR.
    /// </summary>
    [HttpGet("PorAdmissaoCargo/{admissaoCargoId:guid}")]
    public async Task<ActionResult<ApiGenericResult<RemuneracaoCltResult>>> ObterPorAdmissaoCargoId(Guid admissaoCargoId)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ObterPorAdmissaoCargoIdAsync(usuario.Cpf, usuario.OrgId, admissaoCargoId);
        if (!result.Sucesso && result.Retorno == null) return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Cria Remuneração CLT. Requer ADMISSAO_REMUNERACAO_CLT_CRIAR_EDITAR.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiGenericResult<RemuneracaoCltResult>>> Inserir([FromBody] RemuneracaoCltInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.InserirAsync(usuario.Cpf, usuario.OrgId, input);
        if (!result.Sucesso) return BadRequest(result);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Retorno?.Id }, result);
    }

    /// <summary>
    /// Atualiza Remuneração CLT. Requer ADMISSAO_REMUNERACAO_CLT_CRIAR_EDITAR.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<RemuneracaoCltResult>>> Atualizar(Guid id, [FromBody] RemuneracaoCltInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.AtualizarAsync(usuario.Cpf, usuario.OrgId, id, input);
        if (!result.Sucesso && result.Retorno == null) return NotFound(result);
        if (!result.Sucesso) return BadRequest(result);
        return Ok(result);
    }
}
