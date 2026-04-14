using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Reembolso.Solicitacao;

public interface ISolicitacaoReembolsoRepository
{
    Task<List<SolicitacaoReembolsoColaboradorDTO>> ListarPorColabAsync(string codigoInternoColaborador, int orgId, DateTime? dataInicial = null, DateTime? dataFinal = null);
    Task<int> InserirAsync(int orgId, string? projetoId, string? clienteId, int verbaId, string codigoInternoColaborador, string? descricao, DateTime dataDespesa, decimal valor, decimal? valorUnidade, int? quantidade, int statusId, string objetivo, string? destino, DateTime dataInicio, DateTime dataFim);
    Task<SolicitacaoBigNumberDTO> SolicitacaoBigNumbersAsync(string codColaborador, int orgId);
    Task<List<SolicitacaoReembolsoGerenteDTO>> ListarSolicitacoesGerenteProjeto(string? filtro, string? clienteId, string? projetoId, string dataInicio, string dataFim, string codigoColaborador, int orgId, int? statusId);
    Task<List<SolicitacaoColaboradorAprovacaoDTO>> ListarSolicitacoesAprovacaoPorColaborador(string? clienteId, string? projetoId, string codigoGerente, string codigoColaborador, int orgId);
    Task AprovarSolicitacao(int solicitacaoId, string codAprovador, decimal valorAprovado, SolicitacaoStatusEnum status = SolicitacaoStatusEnum.Aprovado);
    Task ReprovarSolicitacao(int solicitacaoId, string codAprovador, string obs);
    Task<SolicitacaoAprovadorDetalheDTO> BuscarSolicitacaoGestorPorId(int solicitacaoId, string codAprovador);
    Task<bool> VerificaSeEhAprovador(string codigoInternoColaborador, int orgId);
    Task<List<SolicitacaoColaboradorVisaoAdmDTO>> ListarSolicitacoesVisaoAdm(int orgId, string? codigoCliente, string? codigoProjeto, string? dataInicio, string? dataFinal, string? codigoAprovador, int? statusId);
    Task<bool> VerificaSeEhGestorAdm(string codigoInternoColaborador, int orgId);
    Task<string> BuscarOperacaoDaSolicitacao(int solicitacaoId);
    Task PagarSolicitacao(int solicitacaoId, string cpfColaboradorPagamento, SolicitacaoStatusEnum status);
    Task<string> BuscarCodigoInternoColaboradorPorExterno(string codigoColaboradorExterno, int orgId);
}