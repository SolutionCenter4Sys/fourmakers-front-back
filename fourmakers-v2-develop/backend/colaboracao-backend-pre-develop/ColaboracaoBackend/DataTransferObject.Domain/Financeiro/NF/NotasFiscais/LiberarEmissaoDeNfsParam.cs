namespace DataTransferObject.Domain.Financeiro.NotaFiscal;

public class LiberarEmissaoDeNfsParam
{
    public int Mes { get; set; }
    public int Ano { get; set; }

    public string? CodigoDiretoria { get; set; }
    public bool EnviarEmail { get; set; }
}