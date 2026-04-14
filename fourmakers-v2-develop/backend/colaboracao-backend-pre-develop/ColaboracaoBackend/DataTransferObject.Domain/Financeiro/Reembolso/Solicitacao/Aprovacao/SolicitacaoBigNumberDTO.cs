namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

public class SolicitacaoBigNumberDTO
{
    public int QtdColaborador { get; set; }
    public decimal ValoresLancados { get; set; } 
    public decimal ValoresAprovados { get; set; }
    public decimal ValoresReprovados { get; set; }
    public decimal ValoresPendentes { get; set; }
}