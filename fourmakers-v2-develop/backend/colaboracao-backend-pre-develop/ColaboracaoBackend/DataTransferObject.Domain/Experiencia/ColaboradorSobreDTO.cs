using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Experiencia
{
    public class ColaboradorSobreDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }
    }
}