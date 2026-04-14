using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Filtro
{
    public class FiltroHobbyDTO
    {
        [JsonIgnore]
        public long filtroId { get; set; }

        [JsonPropertyName("id")]
        public long HobbyId { get; set; }

        [JsonPropertyName("descricao")]
        public string HobbyDescricao { get; set; }
    }
}