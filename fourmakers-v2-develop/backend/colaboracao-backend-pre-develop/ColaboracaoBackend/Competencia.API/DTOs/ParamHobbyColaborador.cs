using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class ParamHobbyColaborador
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }
    }
}