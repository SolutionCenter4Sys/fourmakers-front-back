using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class MotivosDTO
    {
        [JsonPropertyName("reasons_id")]
        public long Reasons_id { get; set; }
        [JsonPropertyName("candidate_joborder_status_id")]
        public int Candidate_joborder_status_id { get; set; }
        [JsonPropertyName("short_description")]
        public string Short_description { get; set; }
    }
}