namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

public class SolicitacaoAprovadorDetalheDTO
{
    public int Id { get; set; }
    public string CodigoColaborador { get; set; }
    public int StatusId { get; set; }
    public bool EhGestorProjeto { get; set; }
    public decimal Valor { get; set; }
    public decimal ValorAprovado { get; set; }
    public int OrgId { get; set; }
    public SolicitacaoStatusEnum Status { get; set; }
}