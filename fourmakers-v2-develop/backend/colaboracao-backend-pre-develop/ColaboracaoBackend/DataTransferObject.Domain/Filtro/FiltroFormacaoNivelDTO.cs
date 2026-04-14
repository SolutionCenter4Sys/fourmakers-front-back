using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class FiltroFormacaoNivelDTO
    {
        [JsonIgnore]
        public long filtroId { get; set; }

        [JsonPropertyName("formacao_id")]
        public long FormacaoId { get; set; }

        [JsonPropertyName("formacaoDescricao")]
        public string FormacaoDescricao { get; set; }

        [JsonPropertyName("nivel_id")]
        public long? NivelId { get; set; }

        [JsonPropertyName("nivelDescricao")]
        public string NivelDescricao { get; set; }
    }
}