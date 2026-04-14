namespace DataTransferObject.Domain.Financeiro.Reembolso.ControleDeSaldo;

public enum StatusSolicitacaoPagamentoEnum : int
{
    AGUARDANDO_PAGAMENTO = 1,
    PAGO = 2,
    ENVIADO_PARA_PAGAMENTO_CNAB = 3
}

public enum TipoMovimentacaoPagamentoEnum : int
{
    SAIDA = 1,
    ENTRADA = 2,
}