using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class LoteFilaRubricaDTO : Apontamento.FolhaPonto.LoteFilaDTO
{
    /// <summary>
    /// Sumário da folha de ponto
    /// </summary>
    [JsonIgnore]
    public string SumarioRubricaCargaString { get; set; }
    public SumarioRubricaCargaDTO SumarioRubricaCarga { get; set; }
}