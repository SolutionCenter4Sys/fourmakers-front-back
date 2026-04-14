using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class ParamGetInteresse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
}