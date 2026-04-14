using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga.AnaliseDocumentosResult;

public class AnaliseAmilResult
{
    public string Titular  { get; set; }
    public string TotalFamilia  { get; set; }
    public string ValorCoparticipacao  { get; set; }
}

public class AnalizeAmilResultWrapper
{
    public List<AnaliseAmilResult> Data { get; set; }
}