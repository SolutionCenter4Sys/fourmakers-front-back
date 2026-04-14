using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.AdmissaoStatus;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.API.Controllers;

/// <summary>
/// CRUD de status de admissão (<c>tb_admissao_status</c>), por organização.
/// </summary>
[Authorize]
[Route("api/Admissao/[controller]")]
[ApiController]
[LogAction]
public class StatusController : ControllerBase
{
    private readonly IAdmissaoStatusService _service;
    private readonly IAspNetUser _aspNetUser;

    public StatusController(IAdmissaoStatusService service, IAspNetUser aspNetUser)
    {
        _service = service;
        _aspNetUser = aspNetUser;
    }

    /// <summary>Lista status de admissão da org. Requer ADMISSAO_STATUS_LISTAR.</summary>
    [HttpGet("Listar")]
    public async Task<ActionResult<ApiGenericResult<List<AdmissaoStatusResult>>>> Listar(
        [FromQuery] bool somenteAtivos = true,
        [FromQuery] int? cursor = null,
        [FromQuery] int? limite = null,
        [FromQuery] string busca = null)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ListarAsync(usuario.Cpf, usuario.OrgId, somenteAtivos, cursor, limite, busca);
        return Ok(result);
    }

    /// <summary>Obtém status por ID. Requer ADMISSAO_STATUS_LISTAR.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoStatusResult>>> ObterPorId(Guid id)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ObterPorIdAsync(usuario.Cpf, usuario.OrgId, id);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        return Ok(result);
    }

    /// <summary>Cria status. Requer ADMISSAO_STATUS_CRIAR_EDITAR.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiGenericResult<AdmissaoStatusResult>>> Inserir([FromBody] AdmissaoStatusInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.InserirAsync(usuario.Cpf, usuario.OrgId, input);
        if (!result.Sucesso)
            return BadRequest(result);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Retorno?.Id }, result);
    }

    /// <summary>Atualiza status. Requer ADMISSAO_STATUS_CRIAR_EDITAR.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoStatusResult>>> Atualizar(Guid id, [FromBody] AdmissaoStatusInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.AtualizarAsync(usuario.Cpf, usuario.OrgId, id, input);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        if (!result.Sucesso)
            return BadRequest(result);
        return Ok(result);
    }

    /// <summary>Inativa o status (exclusão lógica). Requer ADMISSAO_STATUS_CRIAR_EDITAR.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<bool>>> Excluir(Guid id)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ExcluirAsync(usuario.Cpf, usuario.OrgId, id);
        if (!result.Sucesso)
            return NotFound(result);
        return Ok(result);
    }
}
