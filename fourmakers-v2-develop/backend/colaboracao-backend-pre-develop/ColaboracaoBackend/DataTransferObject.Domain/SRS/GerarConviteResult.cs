using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.SRS
{
    public class GerarConviteResult
    {
        [JsonPropertyName("sucess")]
        public bool Sucesso { get; set; }
        [JsonPropertyName("message")]
        public string Mensagem { get; set; }
        [JsonPropertyName("convite")]
        public string Convite { get; set; }
    }
}