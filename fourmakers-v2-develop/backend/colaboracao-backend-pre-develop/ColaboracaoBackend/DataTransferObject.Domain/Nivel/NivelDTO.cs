using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Nivel
{
    public class NivelDTO
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonIgnore]
        public int PrioridadeUnificacao { get; set; }

        [JsonIgnore]
        public int? OrdemExibicao { get; set; }
    }
}