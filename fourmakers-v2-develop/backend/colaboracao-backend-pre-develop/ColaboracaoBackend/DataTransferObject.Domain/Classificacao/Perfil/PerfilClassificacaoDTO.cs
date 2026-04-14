using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Classificacao.Perfil;

public class PerfilClassificacaoDTO
{
    [JsonPropertyName("idVaga")]
    public string IdVaga { get; set; }

    [JsonPropertyName("codVaga")]
    public int? CodVaga { get; set; }

    [JsonPropertyName("titulo")]
    public string Titulo { get; set; }

    [JsonPropertyName("descricao")]
    public string Descricao { get; set; }

    [JsonPropertyName("cargo")]
    public string Cargo { get; set; }

    [JsonPropertyName("skills")]
    public List<PerfilSkillClassificacaoDTO> Skills { get; set; } = new List<PerfilSkillClassificacaoDTO>();
}

