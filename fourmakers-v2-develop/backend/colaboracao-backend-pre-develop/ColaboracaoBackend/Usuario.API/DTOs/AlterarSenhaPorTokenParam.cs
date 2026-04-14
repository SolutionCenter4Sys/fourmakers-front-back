using System.Text.Json.Serialization;

namespace Usuario.API.DTOs
{
    public class AlterarSenhaPorTokenParam
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("senha")]
        public string Senha { get; set; }
    }
}