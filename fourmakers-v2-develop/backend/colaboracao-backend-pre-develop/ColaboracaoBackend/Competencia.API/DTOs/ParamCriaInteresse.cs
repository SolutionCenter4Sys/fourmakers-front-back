using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class ParamCriaInteresse
    {
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}