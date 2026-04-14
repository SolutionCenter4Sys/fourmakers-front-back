using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
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
public class VerbaPersonalizadaController : ControllerBase
{
    private readonly UsuarioLogadoDTO _usuarioLogadoDTO;
    private readonly IVerbaPersonalizadaService _verbaPersonalizadaService;
    
    public VerbaPersonalizadaController(IAspNetUser aspNetUser, IVerbaPersonalizadaService verbaPersonalizadaService)
    {
        _verbaPersonalizadaService = verbaPersonalizadaService;
        _usuarioLogadoDTO = aspNetUser.GetUsuarioLogado();
    }

    [HttpGet("ListarColaboradores")]
    public async Task<ActionResult<ApiGenericResult<List<SimpleColaboradorDTO>>>> ListarColaboradoresAlocadosComOuSemProjetoOuCliente(string? clienteId, string? projetoId)
    {
        return Ok(await _verbaPersonalizadaService.ListarColaboradoresAlocadosPorClienteOuProjeto(_usuarioLogadoDTO.OrgId, clienteId, projetoId));
    }
    
    [HttpGet("ListarVerbasPersonalizadas")]
    public async Task<ActionResult<ApiGenericResult<List<VerbaPersonalizadaCompletaDTO>>>> ListarVerbasPersonalizadas(string? clienteId, string? projetoId)
    {
        return Ok(await _verbaPersonalizadaService.ListarVerbasPersonalizadasPorOrg(_usuarioLogadoDTO.OrgId, clienteId, projetoId));
    }
    
    [HttpPost("InserirVerbasPersonalizadas")]
    public async Task<ActionResult<ApiGenericResult<List<SimpleColaboradorDTO>>>> InserirVerbasPersonalizadas([FromBody] InserirVerbaPersonalizadaParam param)
    {
        return Ok(await _verbaPersonalizadaService.InserirListaDeVerbasPersonalizadas(param, _usuarioLogadoDTO.OrgId));
    }
    
    [HttpPost("InativarVerbaPersonalizada")]
    public async Task<ActionResult<ApiGenericResult<List<SimpleColaboradorDTO>>>> InativarVerbaPersonalizada([FromBody] VerbaPersonalizadaInput param)
    {
        return Ok(await _verbaPersonalizadaService.InativarVerbaPersonalizada(param, _usuarioLogadoDTO.OrgId));
    }
}