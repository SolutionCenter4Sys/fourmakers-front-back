using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.LG.Holerite
{
    public class HoleriteSimplesDTO
    {
        [JsonPropertyName("cpf")]
        public string cpf { get; set; }

        [JsonPropertyName("mes")]
        public int mes { get; set; }

        [JsonPropertyName("ano")]
        public int ano { get; set; }

        [JsonPropertyName("path")]
        public string path { get; set; }

        [JsonPropertyName("emissao")]
        public DateTime emissao { get; set; }
    }
}