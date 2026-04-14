namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga.AnaliseDocumentosResult;

public class AnaliseRubricaCargaResult<T>
{
    public bool Success { get; set; }
    public T Dados { get; set; }
}