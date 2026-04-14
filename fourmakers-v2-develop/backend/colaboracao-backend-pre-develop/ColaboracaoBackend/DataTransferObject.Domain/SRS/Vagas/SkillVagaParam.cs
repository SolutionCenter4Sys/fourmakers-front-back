using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Vagas
{
    public class SkillVagaParam
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("joborder_id")]
        public long Joborder_id { get; set; }

        [JsonPropertyName("skill_id")]
        public int SkillId { get; set; }

        [JsonPropertyName("type_skills_id")]
        public int TypeSkills { get; set; }

        [JsonPropertyName("skill_nivel_id")]
        public int SkillNivelId { get; set; }
    }
}