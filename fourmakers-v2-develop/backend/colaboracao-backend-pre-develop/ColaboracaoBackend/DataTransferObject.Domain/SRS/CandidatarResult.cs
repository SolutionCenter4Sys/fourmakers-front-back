using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class CandidatarResult
    {
        public CandidatarResult()
        {
            Data = new CandidatarSeVagaDTO();
        }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("sucess")]
        public bool Sucess { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("data")]
        public CandidatarSeVagaDTO Data { get; set; }
    }
}