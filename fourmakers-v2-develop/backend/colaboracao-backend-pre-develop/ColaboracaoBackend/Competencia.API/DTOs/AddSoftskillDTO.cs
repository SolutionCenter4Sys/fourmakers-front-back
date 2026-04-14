using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class AddSoftskillDTO
    {
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}