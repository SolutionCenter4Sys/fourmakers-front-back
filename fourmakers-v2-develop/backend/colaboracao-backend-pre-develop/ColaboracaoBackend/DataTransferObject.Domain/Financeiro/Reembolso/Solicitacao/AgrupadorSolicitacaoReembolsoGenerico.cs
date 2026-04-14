using System;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;

public class AgrupadorSolicitacaoReembolsoGenerico <T>
{
    public string NomeColaborador { get; set; }
    /// <summary>Saldo de adiantamentos/reembolso do colaborador (valor disponível para abater em pagamentos).</summary>
    public decimal SaldoAdiantamentos { get; set; }
    public string Objetivo { get; set; }
    public string? Destino { get; set; }
    public string Periodo { get; set; }
    public string Cliente { get; set; }
    public string Projeto { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public decimal SomaValores { get; set; }
    public T Objeto { get; set; }
}