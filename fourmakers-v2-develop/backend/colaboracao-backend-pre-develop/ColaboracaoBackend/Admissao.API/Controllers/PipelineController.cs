using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.AdmissaoPipeline;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.API.Controllers;

/// <summary>
/// CRUD de pipelines (<c>tb_admissao_pipeline</c>) e etapas (<c>tb_admissao_pipeline_status</c>) no mesmo payload (<c>statusItens</c>).
/// </summary>
[Authorize]
[Route("api/Admissao/[controller]")]
[ApiController]
[LogAction]
public class PipelineController : ControllerBase
{
    private readonly IAdmissaoPipelineService _service;
    private readonly IAspNetUser _aspNetUser;

    public PipelineController(IAdmissaoPipelineService service, IAspNetUser aspNetUser)
    {
        _service = service;
        _aspNetUser = aspNetUser;
    }

    /// <summary>Lista pipelines retornando apenas <c>id</c> e <c>nome</c> (sem etapas). Requer ADMISSAO_PIPELINE_LISTAR.</summary>
    [HttpGet("ListarResumido")]
    public async Task<ActionResult<ApiGenericResult<List<AdmissaoPipelineSummaryResult>>>> ListarResumido(
        [FromQuery] bool somenteAtivos = true)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ListarSummaryAsync(usuario.Cpf, usuario.OrgId, somenteAtivos);
        return Ok(result);
    }

    /// <summary>Lista pipelines (cada item inclui <c>statusItens</c>). Requer ADMISSAO_PIPELINE_LISTAR.</summary>
    [HttpGet("Listar")]
    public async Task<ActionResult<ApiGenericResult<List<AdmissaoPipelineResult>>>> Listar(
        [FromQuery] bool somenteAtivos = true,
        [FromQuery] int? cursor = null,
        [FromQuery] int? limite = null,
        [FromQuery] string busca = null)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ListarAsync(usuario.Cpf, usuario.OrgId, somenteAtivos, cursor, limite, busca);
        return Ok(result);
    }

    /// <summary>Obtém pipeline por ID (inclui <c>statusItens</c>). Requer ADMISSAO_PIPELINE_LISTAR.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoPipelineResult>>> ObterPorId(Guid id)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ObterPorIdAsync(usuario.Cpf, usuario.OrgId, id);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        return Ok(result);
    }

    /// <summary>Cria pipeline; opcional <c>statusItens</c> no body. Requer ADMISSAO_PIPELINE_CRIAR_EDITAR.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiGenericResult<AdmissaoPipelineResult>>> Inserir([FromBody] AdmissaoPipelineInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.InserirAsync(usuario.Cpf, usuario.OrgId, input);
        if (!result.Sucesso)
            return BadRequest(result);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Retorno?.Id }, result);
    }

    /// <summary>Atualiza pipeline (body <see cref="AdmissaoPipelineAtualizarInput"/>); <c>statusItens</c> substitui etapas se enviado (<c>null</c> = mantém). Requer ADMISSAO_PIPELINE_CRIAR_EDITAR.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoPipelineResult>>> Atualizar(Guid id, [FromBody] AdmissaoPipelineAtualizarInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.AtualizarAsync(usuario.Cpf, usuario.OrgId, id, input);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        if (!result.Sucesso)
            return BadRequest(result);
        return Ok(result);
    }

    /// <summary>Inativa pipeline (<c>ativo = 0</c>). Requer ADMISSAO_PIPELINE_CRIAR_EDITAR.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<object>>> Inativar(Guid id)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.InativarAsync(usuario.Cpf, usuario.OrgId, id);
        if (!result.Sucesso)
            return NotFound(result);
        return Ok(result);
    }
}
