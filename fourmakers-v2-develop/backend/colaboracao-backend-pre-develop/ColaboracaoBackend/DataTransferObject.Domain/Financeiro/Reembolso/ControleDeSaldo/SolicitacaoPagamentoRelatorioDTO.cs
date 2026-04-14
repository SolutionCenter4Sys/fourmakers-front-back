using System;

namespace DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;

public class SolicitacaoPagamentoRelatorioDTO
{
    public int Id { get; set; }
    public string CodigoColaborador { get; set; }
    public string Nome { get; set; }
    public string Cliente { get; set; }
    public string Projeto { get; set; }
    public DateTime DataDaDespesa { get; set; }
    public DateTime DataDoPedido { get; set; }
    public DateTime DataDaAprovacao { get; set; }
    public string NomeDoAprovador { get; set; }
    public decimal ValorSolicitado { get; set; }
    public decimal ValorAprovado { get; set; }
    public decimal ValorAbatidoDoSaldo { get; set; }
    public decimal ValorParaPagamento { get; set; }
    public bool CustoCliente { get; set; }
    public string Operacao { get; set; }
    public int SolicitacaoReembolsoId { get; set; }
    public string? FormaPagamento { get; set; }
    public string? CodDiretoria { get; set; }
}