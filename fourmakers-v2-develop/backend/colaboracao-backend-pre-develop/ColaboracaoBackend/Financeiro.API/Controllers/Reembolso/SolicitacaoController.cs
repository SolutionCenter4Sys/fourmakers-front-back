using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Reembolso.Solicitacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.Reembolso;

[Authorize]
[Route("api/Financeiro/Reembolso/[controller]")]
[HandleException]
[LogAction]
public class SolicitacaoController : ControllerBase
{
    private readonly ISolicitacaoReembolsoService _solicitacaoService;
    private readonly ISolicitacaoStatusService _solicitacaoStatusService;
    private readonly UsuarioLogadoDTO _usuarioLogado;

    public SolicitacaoController(ISolicitacaoReembolsoService solicitacaoService, IAspNetUser aspNetUser, ISolicitacaoStatusService solicitacaoStatusService)
    {
        _solicitacaoService = solicitacaoService;
        _solicitacaoStatusService = solicitacaoStatusService;
        _usuarioLogado = aspNetUser.GetUsuarioLogado();
    }
    
    [HttpGet("ListarPorColab")]
    public async Task<ActionResult<ApiGenericResult<SolicitacaoColaboradorDetalhesDTO>>> ListarPorColab([FromQuery] DateTime? dataInicial, [FromQuery] DateTime? dataFinal)
    {
        return Ok(await _solicitacaoService.ListarPorColabAsync(_usuarioLogado.Token, _usuarioLogado.Cpf, _usuarioLogado.OrgId, dataInicial, dataFinal));
    }

    [HttpPost("Inserir")]
    [Consumes("application/json")]
    public async Task<ActionResult<ApiGenericResult>> Inserir([FromBody] InserirSolicitacaoReembolsoListaDTO parametro)
    {
        if (parametro == null)
            return BadRequest(new ApiGenericResult { Sucesso = false, Mensagem = "Corpo da requisição inválido." });

        return Ok(await _solicitacaoService.InserirAsync(parametro, _usuarioLogado.OrgId, _usuarioLogado.Cpf));
    }

    [HttpGet("ListarStatus")]
    public async Task<ActionResult<ApiGenericResult<List<SolicitacaoStatusDTO>>>> ListarStatus()
    {
        return Ok(await _solicitacaoStatusService.ListarAsync(_usuarioLogado.OrgId, _usuarioLogado.Cpf));
    }
    
    [HttpPost("ListarSolicitacoesGerenteProjeto")]
    public async Task<ActionResult<ApiGenericResult<List<AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoReembolsoGerenteDTO>>>>>> ListarSolicitacoesGerenteProjeto([FromBody] ListarSolicitacoesGerenteProjetoParam param)
    {
        return Ok(await _solicitacaoService.ListarSolicitacoesPendentes(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId, _usuarioLogado.Token));
    }
    
    [HttpGet("BuscarSolicitacaoBigNumbers")]
    public async Task<ActionResult<ApiGenericResult<SolicitacaoBigNumberDTO>>> BuscarSolicitacaoBigNumbers()
    {
        return Ok(await _solicitacaoService.SolicitacaoBigNumbers(_usuarioLogado.Cpf, _usuarioLogado.OrgId));
    }
    
    [HttpPost("ListarSolicitacoesAprovacaoPorColaborador")]
    public async Task<ActionResult<ApiGenericResult<List<AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoColaboradorAprovacaoDTO>>>>>> ListarSolicitacoesAprovacaoPorColaborador([FromBody] ListarSolicitacoesColaboradorParam param)
    {
        return Ok(await _solicitacaoService.ListarSolicitacoesAprovacaoPorColaborador(_usuarioLogado.Token, _usuarioLogado.Cpf, param.CodigoColaborador, _usuarioLogado.OrgId, param.ClienteId, param.ProjetoId));
    }
    
    [HttpPost("AprovarSolicitacoes")]
    public async Task<ActionResult<ApiGenericResult>> AprovarSolicitacoes([FromBody] AprovacaoSolicitacaoDTO parametro)
    {
        return Ok(await _solicitacaoService.AprovarSolicitacoes(parametro.SolicitacoesIds, _usuarioLogado.Cpf, _usuarioLogado.OrgId));
    }
    
    [HttpPost("ReprovarSolicitacoes")]
    public async Task<ActionResult<ApiGenericResult>> ReprovarSolicitacoes([FromBody] AprovacaoSolicitacaoDTO parametro)
    {
        return Ok(await _solicitacaoService.ReprovarSolicitacoes(parametro.SolicitacoesIds,  parametro.Observacao, _usuarioLogado.Cpf, _usuarioLogado.OrgId));
    }

    [HttpPost("AnalisarComprovantesFiscais")]
    public async Task<ActionResult<ApiGenericResult<AnaliseDocumentoSolicitacaoTotalizadorDTO>>> AnalisarComprovantesFiscais([FromBody] List<Base64DTO> parametros)
    {
        return Ok(await _solicitacaoService.AnalisarComprovantesFiscais(parametros));
    }
    
    [HttpPost("ListarSolicitacoesVisaoAdm")]
    public async Task<ActionResult<ApiGenericResult<SolicitacaoColaboradorVisaoAdmResult>>> ListarSolicitacoesVisaoAdm([FromBody] SolicitacaoColaboradorVisaoAdmParam param)
    {
        return Ok(await _solicitacaoService.ListarSolicitacoesVisaoAdm(param, _usuarioLogado.OrgId, _usuarioLogado.Token));
    }
    
    [HttpPost("GerarReleatorioSolicitacoesAguardandoPagamento")]
    public async Task<FileResult> GerarReleatorioSolicitacoesAguardandoPagamento()
    {
        var fileResult = await _solicitacaoService.GerarReleatorioSolicitacoesAguardandoPagamento(_usuarioLogado.OrgId, _usuarioLogado.Cpf);
        return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);;
    }
    
    [HttpPost("GerarPagamentoDeSolicitacoesDeReembolsoPorListaDeIds")]
    public async Task<ActionResult<ApiGenericResult>> GerarPagamentoDeSolicitacoesDeReembolsoPorListaDeIdsAsync([FromBody]List<int> solicitacoes)
    {
        var result = await _solicitacaoService.GerarPagamentoDeSolicitacoesDeReembolsoPorListaDeIdsAsync(solicitacoes, _usuarioLogado.OrgId, _usuarioLogado.Cpf);
        return Ok(result);
    }
}
