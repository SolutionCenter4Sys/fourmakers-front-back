using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Classificacao.Candidato;

public class CandidatoClassificacaoExperienciaDTO
{
    [JsonIgnore]
    public string CodigoInternoColaborador { get; set; }

    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("titulo")]
    public string Titulo { get; set; }

    [JsonPropertyName("descricao")]
    public string Descricao { get; set; }

    [JsonPropertyName("empresa")]
    public string Empresa { get; set; }

    [JsonPropertyName("dataInicio")]
    public DateTime DataInicio { get; set; }

    [JsonPropertyName("dataSaida")]
    public DateTime? DataSaida { get; set; }

    [JsonPropertyName("atual")]
    public bool Atual { get; set; }
}