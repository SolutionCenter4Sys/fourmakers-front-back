using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class FiltroMetodologiaNivelDTO
    {
        [JsonIgnore]
        public long filtroId { get; set; }

        [JsonPropertyName("metodologia_id")]
        public long MetodologiaId { get; set; }

        [JsonPropertyName("metodologiaDescricao")]
        public string MetodologiaDescricao { get; set; }

        [JsonPropertyName("nivel_id")]
        public long? NivelId { get; set; }

        [JsonPropertyName("nivelDescricao")]
        public string NivelDescricao { get; set; }
    }
}