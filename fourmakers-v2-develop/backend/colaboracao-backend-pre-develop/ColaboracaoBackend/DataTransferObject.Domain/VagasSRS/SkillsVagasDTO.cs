using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.VagasSRS
{
    public class SkillsVagasDTO
    {
        [JsonPropertyName("skill_id")]
        public int SkillId { get; set; }

        [JsonPropertyName("skill_Description")]
        public string SkillDescription { get; set; }

        [JsonPropertyName("type_skills_id")]
        public int TypeSkills { get; set; }

        [JsonPropertyName("type_skills_description")]
        public string TypeSkillsDescription { get; set; }

        [JsonPropertyName("skill_nivel_id")]
        public int SkillNivelId { get; set; }

        [JsonPropertyName("skill_nivel_description")]
        public string SkillNivelDescription { get; set; }

        [JsonIgnore]
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
}