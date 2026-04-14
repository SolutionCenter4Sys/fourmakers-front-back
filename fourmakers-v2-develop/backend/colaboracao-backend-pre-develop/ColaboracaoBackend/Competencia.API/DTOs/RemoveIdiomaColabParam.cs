using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class RemoveIdiomaColabParam
    {
        [JsonPropertyName("id")]
        public int IdiomaId { get; set; }
    }
}