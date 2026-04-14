using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Diretoria
{
    public class DiretoriaDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("diretoria")]
        public string Diretoria { get; set; }

        [JsonPropertyName("id_externo")]
        public string IdExterno { get; set; }
    }
}