using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaboradorLiberacao;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.NotaFiscal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.NotaFiscal;

[Authorize]
[Route("api/Financeiro/NotaFiscal")]
[HandleException]
[ApiController]
[LogAction]
public class NotaFiscalController(INotaFiscalService notaFiscalService, IAspNetUser aspNetUser) : ControllerBase
{
    private readonly UsuarioLogadoDTO _usuarioLogado = aspNetUser.GetUsuarioLogado();
    
    [HttpGet("ListarNotasFiscaisPorVigencia")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<NotaFiscalResult>>>> ListarNotasFiscaisPorVigencia(int? mes, int? ano, string? codDiretoria, string? documentoColaborador, NotaFiscalStatusEnum? statusId, int cursor, int limite)
    {
        var result = await notaFiscalService.ListarNotasFiscaisPorVigenciaAsync(null, statusId,  mes, ano, codDiretoria, documentoColaborador, _usuarioLogado.Cpf, cursor, limite, _usuarioLogado.OrgId, string.Empty);
        return Ok(result);
    }
    
    [HttpGet("ListarNotasFiscaisPorVigenciaVisaoGestor")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<NotaFiscalResult>>>> ListarNotasFiscaisPorVigenciaVisaoGestor(string? filtro, int? mes, int? ano, string? codDiretoria, string? documentoColaborador, NotaFiscalStatusEnum? statusId, int cursor, int limite)
    {
        var result = await notaFiscalService.ListarNotasFiscaisPorVigenciaComColaboradoresSemNFAsync(filtro, statusId,  mes, ano, codDiretoria, null, documentoColaborador, cursor, limite, _usuarioLogado.OrgId, _usuarioLogado.Cpf);
        return Ok(result);
    }
    
    [HttpGet("ListarNotaFiscalStatus")]
    public async Task<ActionResult<ApiGenericResult<IEnumerable<NotaFiscalStatusDTO>>>> ListarNotaFiscalStatus()
    {
        var result = await notaFiscalService.ListarNotaFiscalStatus();
        return Ok(result);
    }
    
    [HttpPost("AprovarNotasFiscais")]
    public async Task<ActionResult<ApiGenericResult>> AprovarNotasFiscais([FromBody] NotaFiscalStatusUpdateParam param)
    {
        var result = await notaFiscalService.AprovarNotasFiscais(param, _usuarioLogado.Cpf,  _usuarioLogado.OrgId);
        return Ok(result);
    }
    
    [HttpPost("ReprovarNotasFiscais")]
    public async Task<ActionResult<ApiGenericResult>> ReprovarNotasFiscais([FromBody] NotaFiscalStatusUpdateParam param)
    {
        var result = await notaFiscalService.ReprovarNotasFiscais(param, _usuarioLogado.Cpf,  _usuarioLogado.OrgId);
        return Ok(result);
    }
    
    [HttpPost("UploadNotaFiscal")]
    public async Task<ActionResult<ApiGenericResult>> UploadNotaFiscal([FromBody] UploadNotaFiscalParam input)
    {
        var result = await notaFiscalService.UploadNotaFiscal(input, _usuarioLogado.Cpf);
        return Ok(result);
    }
    
    [HttpPatch("CancelarEnvioNf/{notaFiscalId}")]
    public async Task<ActionResult<ApiGenericResult>> CancelarEnvioNf(Guid notaFiscalId)
    {
        var result = await notaFiscalService.CancelarEnvioNf(notaFiscalId, _usuarioLogado.Cpf);
        return Ok(result);
    }
    
    [HttpPost("LiberarEmissaoDeNotasFiscaisPorVigencia")]
    public async Task<ActionResult<ApiGenericResult>> LiberarEmissaoDeNotasFiscaisPorVigencia([FromBody] LiberarEmissaoDeNfsParam param)
    {
        var result = await notaFiscalService.LiberarEmissaoDeNotasFiscaisPorVigencia(_usuarioLogado.OrgId, param.Mes, param.Ano, param.CodigoDiretoria, param.EnviarEmail, _usuarioLogado.Cpf);
        return Ok(result);
    }
    
    [HttpGet("ListarRubricasColaboradorParaLiberacaoDeNf")]
    public async Task<ActionResult<ApiGenericResult<List<RubricaColaboradorLiberacaoNfDTO>>>> ListarRubricasColaboradorParaLiberacaoDeNf(int? mes, int? ano)
    {
        var result = await notaFiscalService.ListarRubricasColaboradorParaLiberacaoDeNf(_usuarioLogado.Cpf, mes, ano, _usuarioLogado.OrgId);
        return Ok(result);
    }
    
    [HttpPost("InserirNotaFiscal")]
    public async Task<ActionResult<ApiGenericResult>> InserirNotaFiscal([FromBody] InserirNotaFiscalParam param)
    {
        var result = await notaFiscalService.InserirNotaFiscal(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
        return Ok(result);
    }

    [HttpPost("GerarPagamentoDeNfs")]
    public async Task<ActionResult<ApiGenericResult>> GerarPagamentoDeNfs(NotaFiscalStatusUpdateParam param)
    {
        var result = await notaFiscalService.GerarPagamentoDeSolicitacoesPorIds(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId, true);
        return Ok(result);
    }
}