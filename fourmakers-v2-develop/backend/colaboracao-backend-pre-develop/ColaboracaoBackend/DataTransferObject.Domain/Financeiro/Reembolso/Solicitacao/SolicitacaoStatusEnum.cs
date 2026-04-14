namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;

public enum SolicitacaoStatusEnum : int
{
    Pendente = 1,
    Reprovado = 2,
    Aprovado = 3,
    Pago = 4,
    AprovadoGestor = 5
}