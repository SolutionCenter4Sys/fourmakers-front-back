using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Nivel;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class SkillsDTO
    {
        [JsonPropertyName("skill")]
        public ItemPerfilDTO Skill { get; set; }

        [JsonPropertyName("nivel")]
        public NivelDTO Nivel { get; set; }
    }
}