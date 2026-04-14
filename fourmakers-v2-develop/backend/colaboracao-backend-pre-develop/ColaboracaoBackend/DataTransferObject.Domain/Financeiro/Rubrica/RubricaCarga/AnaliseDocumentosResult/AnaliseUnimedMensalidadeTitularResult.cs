namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga.AnaliseDocumentosResult;

public class AnaliseUnimedMensalidadeTitularResult
{
    public long CpfTitular { get; set; }
    public string NomeTitular { get; set; }
    public decimal Valor { get; set; }
    public string NomeDependente { get; set; }
}