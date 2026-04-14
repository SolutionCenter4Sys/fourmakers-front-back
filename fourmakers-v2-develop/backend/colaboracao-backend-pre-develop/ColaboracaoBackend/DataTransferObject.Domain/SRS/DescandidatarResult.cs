using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class DescandidatarResult
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("sucess")]
        public bool Sucesso { get; set; }
        [JsonPropertyName("message")]
        public string Mensagem { get; set; }
    }
}