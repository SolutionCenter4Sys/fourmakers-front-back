using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class FiltroModeloReferenciaNivelDTO
    {
        [JsonIgnore]
        public long filtroId { get; set; }

        [JsonPropertyName("modelo_id")]
        public long ModeloId { get; set; }

        [JsonPropertyName("modeloDescricao")]
        public string ModeloDescricao { get; set; }

        [JsonPropertyName("nivel_id")]
        public long? NivelId { get; set; }

        [JsonPropertyName("nivelDescricao")]
        public string NivelDescricao { get; set; }
    }
}