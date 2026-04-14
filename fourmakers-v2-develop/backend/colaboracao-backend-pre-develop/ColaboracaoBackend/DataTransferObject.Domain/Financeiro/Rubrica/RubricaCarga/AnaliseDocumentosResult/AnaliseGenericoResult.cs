using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga.AnaliseDocumentosResult;

public class AnaliseGenericoResult
{
    public string Codigo { get; set; }
    public string Valor { get; set; }
    public string Descricao { get; set; }
}

public class AnaliseGenericoResultWrapper
{
    public List<AnaliseGenericoResult> Funcionarios { get; set; }
}

public class AnalizarGenericoProfarmaWrapper
{
    public List<AnaliseGenericoResult> Data { get; set; } 
}