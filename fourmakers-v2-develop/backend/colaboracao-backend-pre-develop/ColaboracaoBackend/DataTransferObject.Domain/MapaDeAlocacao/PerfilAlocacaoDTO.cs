using DataTransferObject.Domain.Competencia;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain
{
    public class PerfilAlocacaoDTO
    {
        [JsonPropertyName("perfil")]
        public string Perfil { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("skills")]
        public List<SkillNivelDTO> Skills { get; set; }
    }
}