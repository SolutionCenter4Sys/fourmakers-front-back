using System.Text.Json.Serialization;

namespace Foursys.API.DTOs
{
    public class ParamAlteraCargo
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }
    }
}