using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Common
{
    public class LogDTO
    {
        [JsonPropertyName("metodo")]
        public string Metodo { get; set; }

        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }

        [JsonPropertyName("stack")]
        public string Stack { get; set; }

        [JsonPropertyName("tipo")]
        public int Tipo { get; set; }

        [JsonPropertyName("group_id")]
        public string GroupId { get; set; }
    }
}