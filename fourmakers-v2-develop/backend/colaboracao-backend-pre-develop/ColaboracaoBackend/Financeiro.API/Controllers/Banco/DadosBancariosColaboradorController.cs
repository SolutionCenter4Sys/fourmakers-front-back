using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Banco.DadosBancariosColaborador;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Banco;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.Banco;

[Authorize]
[Route("api/Financeiro/[controller]")]
[ApiController]
[HandleException]
[LogAction]
public class DadosBancariosColaboradorController : ControllerBase
{
    
    private readonly IDadosBancariosColaboradorService _dadosBancariosColaboradorService;
    private readonly UsuarioLogadoDTO _usuarioLogado;

    public DadosBancariosColaboradorController(
        IDadosBancariosColaboradorService dadosBancariosColaboradorService,
        IAspNetUser aspNetUser)
    {
        _dadosBancariosColaboradorService = dadosBancariosColaboradorService;
        _usuarioLogado = aspNetUser.GetUsuarioLogado();
    }

    [HttpPost("CriarDadosBancarios")]
    public async Task<ActionResult<ApiGenericResult<DadosBancariosColaboradorResult>>> CriarDadosBancarios( [FromBody] DadosBancariosColaboradorBase input)
    {
        var result = await _dadosBancariosColaboradorService.CriarDadosBancariosAsync(input, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
        if (result.Erros != null && result.Erros.Any())
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
    
    [HttpPut("EditarDadosBancarios")]
    public async Task<ActionResult<ApiGenericResult<DadosBancariosColaboradorResult>>> EditarDadosBancarios( [FromBody] DadosBancariosColaboradorBase input)
    {
        var result = await _dadosBancariosColaboradorService.EditarDadosBancariosAsync(input, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
        if (result.Erros != null && result.Erros.Any())
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
    
    [HttpGet("BuscarDadosBancariosPorColaborador")]
    public async Task<ActionResult<ApiGenericResult<DadosBancariosColaboradorResult>>> BuscarDadosBancariosPorColaborador()
    {
        var result = await _dadosBancariosColaboradorService.BuscarDadosBancariosPorColaboradorIdAsync( _usuarioLogado.Cpf, _usuarioLogado.OrgId);
        return Ok(result);
    }
}