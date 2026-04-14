namespace DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;

public class SolicitacaoPagamentoDTO
{
    public int ReembolsoId { get; set; }
    public bool SaldoAbatido { get; set; }
    public decimal Valor { get; set; }
    public StatusSolicitacaoPagamentoEnum Status { get; set; }
}