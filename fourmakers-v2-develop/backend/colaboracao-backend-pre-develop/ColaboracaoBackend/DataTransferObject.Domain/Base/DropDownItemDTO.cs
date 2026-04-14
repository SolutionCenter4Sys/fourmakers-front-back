using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Base
{
    public class DropDownItemDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}