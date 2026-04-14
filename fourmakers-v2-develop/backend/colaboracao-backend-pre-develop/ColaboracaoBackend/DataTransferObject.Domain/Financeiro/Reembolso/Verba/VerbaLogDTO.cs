namespace DataTransferObject.Domain.Financeiro.Reembolso.Verba;

public class VerbaLogDTO
{
    public string Regra { get; set; }
    public string Acao { get; set; }
    public string ValorAnterior { get; set; }
    public string NovoValor { get; set; }
    public string Colaborador { get; set; }
    public string DataCriacao { get; set; }
}