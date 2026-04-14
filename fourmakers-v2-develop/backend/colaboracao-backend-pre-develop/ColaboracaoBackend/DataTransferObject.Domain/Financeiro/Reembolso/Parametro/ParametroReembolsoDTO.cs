namespace DataTransferObject.Domain.Financeiro.Reembolso.Parametro;

public class ParametroReembolsoDTO
{
    public int Id { get; set; }
    public int LimiteEnvio { get; set; }
    public int DiaPagamento { get; set; }
    public int ValidadeComprovanteDias { get; set; }
    public int OrgId { get; set; }
    public int? LimiteEnvioAlternativo { get; set; }
    public int? DiaPagamentoAlternativo { get; set; }
    public bool PermitirAprovarMinhasSolicitacoes { get; set; } = false;
}