using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.VagasSRS
{
    public class SkillsVagasSRSDTO
    {
        [JsonPropertyName("description_id")]
        public int SkillId { get; set; }

        [JsonPropertyName("category_id")]
        public int TypeSkills { get; set; }
        [JsonPropertyName("nivel_id")]
        public int SkillNivelId { get; set; }
        [JsonPropertyName("joborder_id")]
        public int JobOrderId { get; set; }
    }
}