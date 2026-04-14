using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class DesfavoritarResult
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("sucess")]
        public bool Sucess { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}