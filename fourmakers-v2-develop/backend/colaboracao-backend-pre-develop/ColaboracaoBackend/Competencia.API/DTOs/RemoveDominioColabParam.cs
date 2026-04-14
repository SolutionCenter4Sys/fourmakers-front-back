using System.Text.Json.Serialization;

namespace Competencia.API.DTOs
{
    public class RemoveDominioColabParam
    {
        [JsonPropertyName("id")]
        public long DominioId { get; set; }
        public string Cpf { get; set; }
    }
}