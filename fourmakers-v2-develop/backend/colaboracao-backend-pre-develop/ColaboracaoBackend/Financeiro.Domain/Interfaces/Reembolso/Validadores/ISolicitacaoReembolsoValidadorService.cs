using Colaboracao.Helper.Enum;
using Core.Domain.Reembolso.Verba;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

namespace Financeiro.Domain.Interfaces.Reembolso.Validadores;

public interface ISolicitacaoReembolsoValidadorService
{
    Task ValidarSolicitacao(SolicitacaoReembolsoDTO input, CRUDEnum operation, int orgId, string cpfRequest);
    Task<List<string>> GerarListaDeErrosReferenteAComprovante(List<SolicitacaoReembolsoDTO> input, int orgId);
    Task<SolicitacaoAprovadorDetalheDTO> ValidarAprovacao(int solicitacaoId, string codGestor, string? observacao, SolicitacaoStatusEnum status, int orgId);
    void ValidarAgrupador(InserirSolicitacaoReembolsoListaDTO input);
}