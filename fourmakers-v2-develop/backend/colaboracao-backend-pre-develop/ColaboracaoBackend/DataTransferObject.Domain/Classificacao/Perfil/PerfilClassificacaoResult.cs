using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Classificacao.Perfil;

public class PerfilClassificacaoResult
{
    [JsonPropertyName("vagas_classificadas")]
    public List<PerfilClassificadoDTO> VagasClassificadas { get; set; } = new List<PerfilClassificadoDTO>();

    [JsonPropertyName("metricas")]
    public object Metricas { get; set; }

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }
}

public class PerfilClassificadoDTO
{
    [JsonPropertyName("id_vaga")]
    public string IdVaga { get; set; }

    [JsonPropertyName("cod_vaga")]
    public int? CodVaga { get; set; }

    [JsonPropertyName("titulo")]
    public string Titulo { get; set; }

    [JsonPropertyName("cargo")]
    public string Cargo { get; set; }

    [JsonPropertyName("categoria")]
    public string Categoria { get; set; }

    [JsonPropertyName("score_cosseno")]
    public double ScoreCosseno { get; set; }

    [JsonPropertyName("termos_match")]
    public List<string> TermosMatch { get; set; } = new List<string>();
}

