using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Admissao;
using DataTransferObject.Domain.SRS.AdmissaoHistoricoStatus;
using DataTransferObject.Domain.SRS.AdmissaoOrigem;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.API.Controllers;

/// <summary>
/// CRUD de processos de admissão (<c>tb_admissao</c>).
/// Inclui endpoint para movimentação de status com geração automática de histórico.
/// </summary>
[Authorize]
[Route("api/Admissao")]
[ApiController]
[LogAction]
public class AdmissaoController : ControllerBase
{
    private readonly IAdmissaoService _service;
    private readonly IAdmissaoOrigemService _origemService;
    private readonly IAspNetUser _aspNetUser;

    public AdmissaoController(IAdmissaoService service, IAdmissaoOrigemService origemService, IAspNetUser aspNetUser)
    {
        _service = service;
        _origemService = origemService;
        _aspNetUser = aspNetUser;
    }

    /// <summary>Lista processos de admissão da org. Requer ADMISSAO_LISTAR.</summary>
    [HttpGet("Listar")]
    public async Task<ActionResult<ApiGenericResult<List<AdmissaoResult>>>> Listar(
        [FromQuery] bool somenteAtivos = true,
        [FromQuery] int? cursor = null,
        [FromQuery] int? limite = null,
        [FromQuery] Guid? pipelineId = null,
        [FromQuery] Guid? statusId = null)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ListarAsync(usuario.Cpf, usuario.OrgId, somenteAtivos, cursor, limite, pipelineId, statusId);
        return Ok(result);
    }

    /// <summary>Obtém processo de admissão por ID. Requer ADMISSAO_LISTAR.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoResult>>> ObterPorId(Guid id)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ObterPorIdAsync(usuario.Cpf, usuario.OrgId, id);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        return Ok(result);
    }

    /// <summary>Cria processo de admissão. Requer ADMISSAO_CRIAR_EDITAR.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiGenericResult<AdmissaoResult>>> Inserir([FromBody] AdmissaoInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.InserirAsync(usuario.Cpf, usuario.OrgId, input);
        if (!result.Sucesso)
            return BadRequest(result);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Retorno?.Id }, result);
    }

    /// <summary>
    /// Atualiza pipeline, colaborador, observação e ativo de um processo de admissão.
    /// Datas e status são imutáveis por este endpoint: use MoverStatus para alterar o status.
    /// Requer ADMISSAO_CRIAR_EDITAR.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoResult>>> Atualizar(Guid id, [FromBody] AdmissaoUpdateInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.AtualizarAsync(usuario.Cpf, usuario.OrgId, id, input);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        if (!result.Sucesso)
            return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Inativa o processo de admissão (<c>ativo = 0</c>). Requer ADMISSAO_CRIAR_EDITAR.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<bool>>> Inativar(Guid id)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.InativarAsync(usuario.Cpf, usuario.OrgId, id);
        if (!result.Sucesso)
            return NotFound(result);
        return Ok(result);
    }

    /// <summary>
    /// Move o status da admissão, atualizando <c>tb_admissao.tb_admissao_status_id</c>
    /// e registrando a movimentação em <c>tb_admissao_historico_status</c>.
    /// Requer ADMISSAO_CRIAR_EDITAR.
    /// </summary>
    [HttpPost("{id:guid}/MoverStatus")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoHistoricoStatusResult>>> MoverStatus(Guid id, [FromBody] AdmissaoHistoricoStatusInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.MoverStatusAsync(usuario.Cpf, usuario.OrgId, id, input);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        if (!result.Sucesso)
            return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Vincula (ou substitui o vínculo de) um processo de admissão a uma vaga ou candidatura.
    /// Deve ser chamado pelo módulo de recrutamento. Requer ADMISSAO_CRIAR_EDITAR.
    /// </summary>
    [HttpPost("{id:guid}/VincularOrigem")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoOrigemResult>>> VincularOrigem(Guid id, [FromBody] AdmissaoOrigemInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _origemService.VincularAsync(usuario.Cpf, usuario.OrgId, id, input);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        if (!result.Sucesso)
            return BadRequest(result);
        return Ok(result);
    }

    /// <summary>
    /// Retorna o vínculo de origem de um processo de admissão (vaga ou candidatura).
    /// Retorno <c>null</c> quando a admissão não possui vínculo (cliente sem módulo de recrutamento).
    /// Requer ADMISSAO_LISTAR.
    /// </summary>
    [HttpGet("{id:guid}/Origem")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoOrigemResult>>> ObterOrigem(Guid id)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _origemService.ObterVinculoAsync(usuario.Cpf, usuario.OrgId, id);
        return Ok(result);
    }
}
