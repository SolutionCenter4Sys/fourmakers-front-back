using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Rubrica.RubricaCarga;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.Rubrica;

[Authorize]
[Route("api/Financeiro/Rubrica/Carga")]
[HandleException]
[ApiController]
[LogAction]
public class RubricaCargaController : ControllerBase
{
    private readonly IRubricaCargaService _rubricaCargaService;
    private readonly UsuarioLogadoDTO _usuarioLogado;

    public RubricaCargaController(IRubricaCargaService rubricaCargaService, IAspNetUser aspNetUser)
    {
        _rubricaCargaService = rubricaCargaService;
        _usuarioLogado = aspNetUser.GetUsuarioLogado();
    }

    [HttpPost("InserirCarga")]
    public async Task<ActionResult<ApiGenericResult<RubricaCargaStatusDTO>>> InserirCarga(RubricaCargaInput input)
    {
        var result = await _rubricaCargaService.InserirCarga(input, _usuarioLogado.Cpf, _usuarioLogado.Token, _usuarioLogado.OrgId);
        return Ok(result);
    }
    
    [HttpPost("InserirCargaV2")]
    public async Task<ActionResult<ApiGenericResult<RubricaCargaStatusDTO>>> IniciarProcessamentoDeCarga(RubricaCargaInput input)
    {
        var result = await _rubricaCargaService.CriarSumarioRubricaCarga(input, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
        return Ok(result);
    }
        
    [HttpGet("ObterStatusCargaPorId")]
    public async Task<ActionResult<ApiGenericResult<RubricaCargaResult>>> Obter([FromQuery] string cargaId)
    {
        var result = await _rubricaCargaService.ObterStatusCargaPorId(cargaId);
        return Ok(result);
    }
    
    [HttpGet("ListarHistoricoDeCargaRecentes")]
    public async Task<ActionResult<ApiGenericResult<List<RubricaCargaStatusDTO>>>> ListarHistoricoDeCargaRecentes()
    {
        var result = await _rubricaCargaService.ListarHistoricoDeCargaRecentes(_usuarioLogado.OrgId, _usuarioLogado.Cpf);
        return Ok(result);
    }
    
    [HttpGet("ListarCargasPorOrg/{codigoDiretoria}/{rubricaId}/{mes}/{ano}")]
    public async Task<ActionResult<ApiGenericResult<List<RubricaCargaStatusDTO>>>> ListarCargasPorOrg(string codigoDiretoria, string rubricaId, int mes, int ano)
    {
        var result = await _rubricaCargaService.ListarCargasPorOrg(_usuarioLogado.OrgId, codigoDiretoria, rubricaId, mes, ano);
        return Ok(result);
    }
    
    [HttpGet("ObterCargaPorCodigoCarga/{codigoCarga}")]
    public async Task<ActionResult<ApiGenericResult<RubricaCargaStatusDTO>>> ObterCargaPorCodigoCarga(string codigoCarga)
    {
        var result = await _rubricaCargaService.ObterCargaPorCodigoCarga(codigoCarga);
        return Ok(result);
    }
    
    [HttpGet("ObterPDFCargaPorMesEAno/{orgId}/{codigoInternoColaborador}/{mes}/{ano}")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<RubricaCargaOrigemPdfDTO>>>> ObterPDFCargaPorMesEAno(int orgId, string codigoInternoColaborador, int mes, int ano)
    {
        var result = await _rubricaCargaService.ObterPDFCargaPorMesEAno(orgId, codigoInternoColaborador, mes, ano);
        return Ok(result);
    }
}