using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class NacionalidadeDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}