namespace DataTransferObject.Domain.Financeiro.Reembolso.Verba;

public class VerbaDTO
{
    public int Id { get; set; }
    public string Categoria { get; set; }
    public int TipoCusto { get; set; }
    public string TipoCustoDescricao { get; set; }
    public string Unidade { get; set; }
    public decimal Valor { get; set; }
    public bool CustoCliente { get; set; }
    public bool Ativo { get; set; }
    public VerbaTipoCustoEnum TipoCodigo { get; set; }
    public bool ExigirComprovante { get; set; }
}