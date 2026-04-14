using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga.AnaliseDocumentosResult;

public class AnalisePortoSeguroOdontoResult
{
    public string Codigo { get; set; }
    public string Nome { get; set; }
    public string Cpf  { get; set; }
    public string Rubrica  { get; set; }
    public string Valor { get; set; }
}

public class AnalisePortoSeguroOdontoResultWrapper
{
    public List<AnalisePortoSeguroOdontoResult> Data { get; set; }
}