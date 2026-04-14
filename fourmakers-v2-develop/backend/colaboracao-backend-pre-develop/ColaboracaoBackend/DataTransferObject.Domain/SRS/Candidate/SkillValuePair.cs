using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS.Candidate
{
    public class SkillValuePair
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}