using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class AddSoftskillParam
    {
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}