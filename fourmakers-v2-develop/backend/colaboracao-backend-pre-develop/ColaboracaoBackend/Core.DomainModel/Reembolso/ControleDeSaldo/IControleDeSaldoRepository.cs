using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;

namespace Core.Domain.Reembolso.ControleDeSaldo;

public interface IControleDeSaldoRepository
{
    Task<decimal> BuscarSaldoColaborador(string codigoInternoColaborador, int orgId);
    Task<Dictionary<string, decimal>> BuscarSaldosColaboradores(IEnumerable<string> codigosColaboradores, int orgId);
    Task EditarValorSaldoColaborador(string codigoInternoColaborador, int orgId, decimal valor);

    Task<int> CriarSolicitacaoDePagamento(StatusSolicitacaoPagamentoEnum status, string codigoInternoColaborador,
        int orgId, int solicitacaoReembolsoId, string origem, decimal valor, bool usouSaldo);    Task GerarExtratoPagamento(TipoMovimentacaoPagamentoEnum tipoMovimentacao, int solicitacaoPagamentoId, int orgId, decimal valor, string codigoInternoColaborador);
    Task EditarStatusPagamento(int id, StatusSolicitacaoPagamentoEnum status, bool usouSaldo);
    Task AtualizarApenasStatusPagamento(int id, StatusSolicitacaoPagamentoEnum status);
    Task<List<SolicitacaoPagamentoRelatorioDTO>> BuscarSolicitacoesPagamentoPorStatusRelatorio(StatusSolicitacaoPagamentoEnum status, int orgId, string competencia = "", string codDiretoria = "");
    Task<List<SolicitacaoPagamentoDTO>> BuscarSolicitacoesPagamentos(int orgId);
    Task<List<SolicitacaoPagamentoRelatorioDTO>> BuscarSolicitacoesPagamentoPorIds(List<int> ids, int orgId);
    Task<List<SolicitacaoPagamentoRelatorioDTO>> BuscarSolicitacoesPagamentoPorListaDeSolicitacaoDeReembolsoIds(List<int> ids, int orgId);
}