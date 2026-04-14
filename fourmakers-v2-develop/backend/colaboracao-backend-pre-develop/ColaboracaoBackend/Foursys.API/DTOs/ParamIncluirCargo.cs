using System.Text.Json.Serialization;

namespace Foursys.API.DTOs
{
    public class ParamIncluirCargo
    {
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}