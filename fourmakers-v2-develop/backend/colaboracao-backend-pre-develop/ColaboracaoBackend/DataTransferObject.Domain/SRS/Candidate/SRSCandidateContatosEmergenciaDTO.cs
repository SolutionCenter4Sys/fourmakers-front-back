using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class SRSCandidateContatosEmergenciaDTO
    {
        [JsonIgnore]
        public int display_order { get; set; }

        [JsonPropertyName("contact_order")]
        public int contact_order { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("degree_kinship")]
        public string degree_kinship { get; set; }

        [JsonPropertyName("phone")]
        public string phone { get; set; }
    }
}