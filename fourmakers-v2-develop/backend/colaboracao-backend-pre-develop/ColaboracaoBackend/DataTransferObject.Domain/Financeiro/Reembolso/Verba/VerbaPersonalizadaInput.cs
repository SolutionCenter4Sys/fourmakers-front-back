namespace DataTransferObject.Domain.Financeiro.Reembolso.Verba;

public class VerbaPersonalizadaInput
{
    public int? Id { get; set; }
    public int VerbaId { get; set; }
    public string ClienteId { get; set; } = string.Empty;
    public string ProjetoId { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public bool CustoCliente { get; set; }
    public decimal Valor { get; set; }
    public string CodigoInternoColaborador { get; set; } = string.Empty;
}