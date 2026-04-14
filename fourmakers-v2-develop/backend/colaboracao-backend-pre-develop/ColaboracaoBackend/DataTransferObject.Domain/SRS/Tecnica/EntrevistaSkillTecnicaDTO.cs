using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Tecnica
{
    public class EntrevistaSkillTecnicaDTO
    {
        [JsonIgnore]
        public int Tecnica_id { get; set; }
        [JsonPropertyName("candidate_id")]
        public int Candidate_id { get; set; }
        [JsonPropertyName("category_id")]
        public int Category_id { get; set; }
        [JsonPropertyName("description_id")]
        public int Description_id { get; set; }
        [JsonPropertyName("nivel_id")]
        public int Nivel_id { get; set; }
    }
}