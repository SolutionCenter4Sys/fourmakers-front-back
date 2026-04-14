using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Classificacao.Perfil;

public class PerfilSkillClassificacaoDTO
{
    [JsonPropertyName("skillId")]
    public long SkillId { get; set; }

    [JsonPropertyName("nivelId")]
    public int NivelId { get; set; }

    [JsonPropertyName("itemPerfilId")]
    public int ItemPerfilId { get; set; }

    [JsonPropertyName("relevante")]
    public bool Relevante { get; set; }
}

