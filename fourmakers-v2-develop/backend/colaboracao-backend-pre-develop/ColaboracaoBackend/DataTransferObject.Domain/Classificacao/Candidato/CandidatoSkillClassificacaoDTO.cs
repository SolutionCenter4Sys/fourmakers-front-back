using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Classificacao.Candidato;

public class CandidatoSkillClassificacaoDTO
{
    [JsonIgnore]
    public string CodigoInternoColaborador { get; set; }
    [JsonPropertyName("nome")]
    public string Nome { get; set; }
}