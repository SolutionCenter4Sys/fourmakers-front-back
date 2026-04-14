using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class DescandidatarSeParam
    {
        [JsonPropertyName("joborder_id")]
        public long joborder_id { get; set; }
        [JsonPropertyName("status_reason_cancellation")]
        public int status_reason_cancellation { get; set; }
        [JsonPropertyName("reason_cancellation")]
        public int reason_cancellation { get; set; }
    }
}