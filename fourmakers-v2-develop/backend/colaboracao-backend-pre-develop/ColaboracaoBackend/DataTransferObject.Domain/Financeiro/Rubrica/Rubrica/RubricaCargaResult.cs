using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;

public class RubricaCargaResult : RubricaCargaBase
{
    [JsonPropertyName("mensagem")]
    public string Mensagem { get; set; }
}