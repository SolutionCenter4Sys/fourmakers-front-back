using DataTransferObject.Domain.Arquivo;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;
using Microsoft.AspNetCore.Mvc;

namespace Financeiro.Domain.Interfaces.Reembolso.Solicitacao;

public interface ISolicitacaoReembolsoService
{
    Task<ApiGenericResult<SolicitacaoColaboradorDetalhesDTO>> ListarPorColabAsync(string tokenUsuario, string codigoInternoColaborador, int orgId, DateTime? dataInicial = null, DateTime? dataFinal = null);
    Task<ApiGenericResult> InserirAsync(InserirSolicitacaoReembolsoListaDTO parametro,  int orgId, string codigoInternoColaborador);
    Task<ApiGenericResult<SolicitacaoBigNumberDTO>> SolicitacaoBigNumbers(string codColaborador, int orgId);
    Task<ApiGenericResult<List<AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoReembolsoGerenteDTO>>>>> ListarSolicitacoesPendentes(ListarSolicitacoesGerenteProjetoParam param, string codColaborador, int orgId,string tokenUsuario);
    Task<ApiGenericResult<List<AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoColaboradorAprovacaoDTO>>>>> ListarSolicitacoesAprovacaoPorColaborador(string tokenUsuario, string codGerente, string codColaborador, int orgId, string? clienteId, string? projetoId);
    Task<ApiGenericResult> AprovarSolicitacoes(List<int> solicitacoesId, string codAprovador, int orgId);
    Task<ApiGenericResult> ReprovarSolicitacoes(List<int> solicitacoesId, string observacao, string codAprovador, int orgId);
    Task<ApiGenericResult<AnaliseDocumentoSolicitacaoTotalizadorDTO>> AnalisarComprovantesFiscais(List<Base64DTO> documentos);
    Task<ApiGenericResult<SolicitacaoColaboradorVisaoAdmResult>> ListarSolicitacoesVisaoAdm(SolicitacaoColaboradorVisaoAdmParam param, int orgId, string tokenUsuario);
    Task<ApiGenericResult<FileContentResult>> GerarReleatorioSolicitacoesAguardandoPagamento(int orgId, string cpfRequest);
    Task GerarPagamentoDeSolicitacoesPorIds(List<int> ids, int orgId, string cpfRequest);
    Task<ApiGenericResult> GerarPagamentoDeSolicitacoesDeReembolsoPorListaDeIdsAsync(List<int> ids, int orgId, string cpfRequest);
}