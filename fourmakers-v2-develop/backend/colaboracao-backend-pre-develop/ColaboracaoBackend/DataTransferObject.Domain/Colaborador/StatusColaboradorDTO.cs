using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class StatusColaboradorDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}