using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class TotalizadoresDTO
    {
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("total")]
        public long Total { get; set; }
    }
}