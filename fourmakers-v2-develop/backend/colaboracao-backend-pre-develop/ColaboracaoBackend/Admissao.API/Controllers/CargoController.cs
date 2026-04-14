using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.AdmissaoCargo;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Admissao.Domain.Interfaces.Service;

namespace Admissao.API.Controllers;

/// <summary>
/// CRUD de cargos de admissão (dados em <c>tb_admissao_cargo</c>).
/// Criar/editar exige <c>cboId</c> de um CBO existente em <c>tb_admissao_cbo</c>.
/// </summary>
[Authorize]
[Route("api/Admissao/[controller]")]
[ApiController]
[LogAction]
public class CargoController : ControllerBase
{
    private readonly IAdmissaoCargoService _service;
    private readonly IAspNetUser _aspNetUser;

    public CargoController(IAdmissaoCargoService service, IAspNetUser aspNetUser)
    {
        _service = service;
        _aspNetUser = aspNetUser;
    }

    /// <summary>Lista cargos de admissão. Requer ADMISSAO_CARGO_LISTAR.</summary>
    [HttpGet("Listar")]
    public async Task<ActionResult<ApiGenericResult<List<AdmissaoCargoResult>>>> Listar(
        [FromQuery] bool somenteAtivos = true,
        [FromQuery] int? cursor = null,
        [FromQuery] int? limite = null,
        [FromQuery] string busca = null)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ListarAsync(usuario.Cpf, usuario.OrgId, somenteAtivos, cursor, limite, busca);
        return Ok(result);
    }

    /// <summary>Lista cargos da org que possuem registro em tb_admissao_remuneracao_clt. Mesmos filtros de Listar. Requer ADMISSAO_CARGO_LISTAR.</summary>
    [HttpGet("ListarComRemuneracaoCadastrada")]
    public async Task<ActionResult<ApiGenericResult<List<AdmissaoCargoResult>>>> ListarComRemuneracaoCadastrada(
        [FromQuery] bool somenteAtivos = true,
        [FromQuery] int? cursor = null,
        [FromQuery] int? limite = null,
        [FromQuery] string busca = null)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ListarComRemuneracaoCadastradaAsync(usuario.Cpf, usuario.OrgId, somenteAtivos, cursor, limite, busca);
        return Ok(result);
    }

    /// <summary>Obtém cargo de admissão por ID. Requer ADMISSAO_CARGO_LISTAR.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoCargoResult>>> ObterPorId(Guid id)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.ObterPorIdAsync(usuario.Cpf, usuario.OrgId, id);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        return Ok(result);
    }

    /// <summary>Cria cargo de admissão vinculado a um CBO (<c>cboId</c>). Requer ADMISSAO_CARGO_CRIAR_EDITAR.</summary>
    [HttpPost]
    public async Task<ActionResult<ApiGenericResult<AdmissaoCargoResult>>> Inserir([FromBody] AdmissaoCargoInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.InserirAsync(usuario.Cpf, usuario.OrgId, input);
        if (!result.Sucesso)
            return BadRequest(result);
        return CreatedAtAction(nameof(ObterPorId), new { id = result.Retorno?.Id }, result);
    }

    /// <summary>Atualiza cargo de admissão (mantém vínculo obrigatório com CBO via <c>cboId</c>). Requer ADMISSAO_CARGO_CRIAR_EDITAR.</summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiGenericResult<AdmissaoCargoResult>>> Atualizar(Guid id, [FromBody] AdmissaoCargoInput input)
    {
        var usuario = _aspNetUser.GetUsuarioLogado();
        var result = await _service.AtualizarAsync(usuario.Cpf, usuario.OrgId, id, input);
        if (!result.Sucesso && result.Retorno == null)
            return NotFound(result);
        if (!result.Sucesso)
            return BadRequest(result);
        return Ok(result);
    }
}
