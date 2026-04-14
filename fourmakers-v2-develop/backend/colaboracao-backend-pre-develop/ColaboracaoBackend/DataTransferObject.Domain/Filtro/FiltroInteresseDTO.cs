using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class FiltroInteresseDTO
    {
        [JsonIgnore]
        public long filtroId { get; set; }

        [JsonPropertyName("id")]
        public long InteresseId { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}