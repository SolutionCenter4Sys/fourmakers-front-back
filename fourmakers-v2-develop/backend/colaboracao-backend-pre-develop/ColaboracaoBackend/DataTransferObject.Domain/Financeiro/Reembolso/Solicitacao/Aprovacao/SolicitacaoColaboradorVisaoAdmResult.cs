using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

public class SolicitacaoColaboradorVisaoAdmResult
{
    public List<AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoColaboradorVisaoAdmDTO>>> Solicitacoes { get; set; }
    public int Colaboradores { get; set; } 
    public decimal ValorSolicitado { get; set; }
    public decimal ValorAprovado { get; set; }
    public decimal ValorReprovado { get; set; }
    public decimal ValorPendente { get; set; }
    public decimal ValorPago { get; set; }
}