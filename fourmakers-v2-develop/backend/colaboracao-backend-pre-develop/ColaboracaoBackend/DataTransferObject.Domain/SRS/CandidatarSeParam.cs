using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class CandidatarSeParam
    {
        [JsonPropertyName("joborder_id")]
        public long joborder_id { get; set; }
        [JsonPropertyName("convite")]
        public string convite { get; set; }
    }
}