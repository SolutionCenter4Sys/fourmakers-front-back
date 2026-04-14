using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class FiltroDominioNivelDTO
    {
        [JsonIgnore]
        public long filtroId { get; set; }

        [JsonPropertyName("dominio_id")]
        public long DominioId { get; set; }

        [JsonPropertyName("dominioDescricao")]
        public string DominioDescricao { get; set; }

        [JsonPropertyName("nivel_id")]
        public long? NivelId { get; set; }

        [JsonPropertyName("nivelDescricao")]
        public string NivelDescricao { get; set; }
    }
}