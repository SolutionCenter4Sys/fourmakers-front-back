using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class ParamCriaHobby
    {
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}