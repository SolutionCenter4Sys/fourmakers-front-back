using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;

public class SolicitacaoColaboradorDetalhesDTO
{
    public List<AgrupadorSolicitacaoReembolsoGenerico<List<SolicitacaoReembolsoColaboradorDTO>>> Solicitacoes { get; set; }
    public decimal TotalSolicitado  { get; set; }
    public decimal TotalAprovado  { get; set; }
    public bool SouAprovador  { get; set; }
    public bool SouGestor { get; set; }
    public decimal Saldo { get; set; }
}