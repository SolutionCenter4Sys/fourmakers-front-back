using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Classificacao.Candidato;

public class CandidatoClassificacaoResult
{
    [JsonPropertyName("candidatos_classificados")]
    public List<CandidatoClassificadoDTO> CandidatoClassificados { get; set; }
}

public class CandidatoClassificadoDTO
{
    [JsonPropertyName("id_colaborador")]
    public string IdColaborador { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    [JsonPropertyName("score_cosseno")]
    public double ScoreCosseno { get; set; }
}