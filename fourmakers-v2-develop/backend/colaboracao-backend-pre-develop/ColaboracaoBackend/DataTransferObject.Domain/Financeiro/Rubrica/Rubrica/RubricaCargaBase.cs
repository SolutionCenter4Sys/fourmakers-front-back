using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;

public class RubricaCargaBase
{
    [JsonPropertyName("codigo_carga_rubrica")]
    public string CodigoCargaRubrica { get; set; }
    [JsonPropertyName("status")]
    public string Status { get; set; }
}