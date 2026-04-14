using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeRelacionamento.VCX;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeRelacionamento.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace MapaDeRelacionamento.API.Controllers;

[HandleException]
[Route("api/MapaDeRelacionamento/[controller]")]
[ApiController]
[Authorize]
[LogAction]
public class VCXController : ControllerBase
{
    private readonly IVCXService _vcxService;
    private readonly UsuarioLogadoDTO _usuarioLogado;

    public VCXController(IVCXService vcxService, IAspNetUser aspNetUser)
    {
        _vcxService = vcxService;
        _usuarioLogado = aspNetUser.GetUsuarioLogado();
    }

    // ---- Dores por posição ----
    [HttpGet("ListarDoresPorPosicaoId")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<VCXDorDTO>>>> ListarDoresPorPosicaoId([FromQuery] Guid posicaoId)
        => Ok(await _vcxService.GetDoresByPosicaoIdAsync(posicaoId));

    [HttpGet("BuscarDorPorId")]
    public async Task<ActionResult<ApiGenericResult<VCXDorDTO>>> BuscarDorPorId([FromQuery] Guid id)
        => Ok(await _vcxService.GetDorByIdAsync(id));

    [HttpPost("CriarDor")]
    public async Task<ActionResult<ApiGenericResult<VCXDorDTO>>> CriarDor([FromBody] VCXDorInputDTO input)
        => Ok(await _vcxService.CreateDorAsync(input, _usuarioLogado.Cpf));

    [HttpPut("AtualizarDor")]
    public async Task<ActionResult<ApiGenericResult<VCXDorDTO>>> AtualizarDor([FromBody] VCXDorInputDTO input)
        => Ok(await _vcxService.UpdateDorAsync(input, _usuarioLogado.Cpf));

    [HttpDelete("ExcluirDor")]
    public async Task<ActionResult<ApiGenericResult>> ExcluirDor([FromQuery] Guid id)
        => Ok(await _vcxService.DeleteDorAsync(id, _usuarioLogado.Cpf));

    // ---- Iniciativas por posição ----
    [HttpGet("ListarIniciativasPorPosicaoId")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<VCXIniciativaDTO>>>> ListarIniciativasPorPosicaoId([FromQuery] Guid posicaoId)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.GetIniciativasByPosicaoIdAsync(posicaoId, orgId));
    }

    [HttpGet("BuscarIniciativaPorId")]
    public async Task<ActionResult<ApiGenericResult<VCXIniciativaDTO>>> BuscarIniciativaPorId([FromQuery] Guid id)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.GetIniciativaByIdAsync(id, orgId));
    }

    [HttpPost("CriarIniciativa")]
    public async Task<ActionResult<ApiGenericResult<VCXIniciativaDTO>>> CriarIniciativa([FromBody] VCXIniciativaInputDTO input)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.CreateIniciativaAsync(input, _usuarioLogado.Cpf, orgId));
    }

    [HttpPut("AtualizarIniciativa")]
    public async Task<ActionResult<ApiGenericResult<VCXIniciativaDTO>>> AtualizarIniciativa([FromBody] VCXIniciativaInputDTO input)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.UpdateIniciativaAsync(input, _usuarioLogado.Cpf, orgId));
    }

    [HttpDelete("ExcluirIniciativa")]
    public async Task<ActionResult<ApiGenericResult>> ExcluirIniciativa([FromQuery] Guid id)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.DeleteIniciativaAsync(id, _usuarioLogado.Cpf, orgId));
    }

    // ---- Notas de bastidores por posição ----
    [HttpGet("ListarNotasBastidoresPorPosicaoId")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<VCXNotaBastidoresDTO>>>> ListarNotasBastidoresPorPosicaoId([FromQuery] Guid posicaoId)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.GetNotasBastidoresByPosicaoIdAsync(posicaoId, orgId));
    }

    [HttpGet("BuscarNotaBastidoresPorId")]
    public async Task<ActionResult<ApiGenericResult<VCXNotaBastidoresDTO>>> BuscarNotaBastidoresPorId([FromQuery] Guid id)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.GetNotaBastidoresByIdAsync(id, orgId));
    }

    [HttpPost("CriarNotaBastidores")]
    public async Task<ActionResult<ApiGenericResult<VCXNotaBastidoresDTO>>> CriarNotaBastidores([FromBody] VCXNotaBastidoresCriarInputDTO input)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.CreateNotaBastidoresAsync(input, _usuarioLogado.Cpf, orgId));
    }

    [HttpPut("AtualizarNotaBastidores")]
    public async Task<ActionResult<ApiGenericResult<VCXNotaBastidoresDTO>>> AtualizarNotaBastidores([FromBody] VCXNotaBastidoresAtualizarInputDTO input)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.UpdateNotaBastidoresAsync(input, _usuarioLogado.Cpf, orgId));
    }

    [HttpDelete("ExcluirNotaBastidores")]
    public async Task<ActionResult<ApiGenericResult>> ExcluirNotaBastidores([FromQuery] Guid id)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.DeleteNotaBastidoresAsync(id, _usuarioLogado.Cpf, orgId));
    }

    [HttpGet("ListarHistoricoNotasBastidoresPorPosicaoId")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<VCXNotaBastidoresLogDTO>>>> ListarHistoricoNotasBastidoresPorPosicaoId([FromQuery] Guid posicaoId)
        => Ok(await _vcxService.GetHistoricoNotasBastidoresByPosicaoIdAsync(posicaoId));

    // ---- Dores + Iniciativas em conjunto ----
    [HttpGet("ListarDoresIniciativasPorPosicaoId")]
    public async Task<ActionResult<ApiGenericResult<VCXPosicaoDoresIniciativasResultDTO>>> ListarDoresIniciativasPorPosicaoId([FromQuery] Guid posicaoId)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.GetDoresAndIniciativasByPosicaoIdAsync(posicaoId, orgId));
    }

    [HttpGet("ListarHistoricoDoresIniciativasPorPosicaoId")]
    public async Task<ActionResult<ApiGenericResult<VCXHistoricoDoresIniciativasResultDTO>>> ListarHistoricoDoresIniciativasPorPosicaoId([FromQuery] Guid posicaoId)
        => Ok(await _vcxService.GetHistoricoDoresIniciativasByPosicaoIdAsync(posicaoId));

    // ---- Temas (por org do usuário logado) ----
    [HttpGet("ListarTemas")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<VCXTemaDTO>>>> ListarTemas([FromQuery] string? descricao = null)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.GetTemasByOrgIdAsync(orgId, descricao));
    }

    [HttpPost("CriarTema")]
    public async Task<ActionResult<ApiGenericResult<VCXTemaDTO>>> CriarTema([FromBody] VCXTemaInputDTO input)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.CreateTemaAsync(input, orgId));
    }

    // ---- Listagens de dados de referência ----
    [HttpGet("ListarImpactosDores")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<VCXImpactoDTO>>>> ListarImpactosDores()
        => Ok(await _vcxService.GetImpactosAsync());

    [HttpGet("ListarUrgenciasDores")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<VCXUrgenciaDTO>>>> ListarUrgenciasDores()
        => Ok(await _vcxService.GetUrgenciasAsync());

    [HttpGet("ListarStatusIniciativas")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<VCXStatusDTO>>>> ListarStatusIniciativas()
        => Ok(await _vcxService.GetStatusAsync());

    [HttpGet("ListarAgendasPorColaboradorCliente")]
    public async Task<ActionResult<ApiGenericResult<VCXAgendasPorColaboradorClienteResultDTO>>> ListarAgendasPorColaboradorCliente([FromQuery] string codigoColaborador, [FromQuery] string codigoCliente, [FromQuery] int limit, [FromQuery] int cursor = 0)
        => Ok(await _vcxService.GetAgendasPorColaboradorClienteAsync(codigoColaborador, codigoCliente, cursor, limit));

    // ---- Histórico de orçamento por posição ----
    [HttpGet("ListarOrcamentoHistoricoPorPosicaoId")]
    public async Task<ActionResult<ApiGenericResult<List<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>>>> ListarOrcamentoHistoricoPorPosicaoId([FromQuery] Guid posicaoId)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.ListarOrcamentoHistoricoPorPosicaoIdAsync(posicaoId, orgId));
    }

    [HttpGet("BuscarUltimoOrcamentoPorPosicaoId")]
    public async Task<ActionResult<ApiGenericResult<OrganogramaPosicaoOrcamentoHistoricoResponseDTO?>>> BuscarUltimoOrcamentoPorPosicaoId([FromQuery] Guid posicaoId)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.BuscarUltimoOrcamentoPorPosicaoIdAsync(posicaoId, orgId));
    }

    [HttpPost("CriarOrcamentoHistoricoPorPosicao")]
    public async Task<ActionResult<ApiGenericResult<OrganogramaPosicaoOrcamentoHistoricoResponseDTO>>> CriarOrcamentoHistoricoPorPosicao([FromQuery] Guid posicaoId, [FromBody] OrganogramaPosicaoOrcamentoHistoricoInserirParamDTO param)
    {
        var orgId = _usuarioLogado.OrgId;
        return Ok(await _vcxService.CriarOrcamentoHistoricoPorPosicaoAsync(posicaoId, param, _usuarioLogado.Cpf, orgId));
    }
}
