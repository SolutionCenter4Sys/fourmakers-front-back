using System.Text.Json.Serialization;

namespace Firebase.API.DTOs
{
    public class ParamFirebase
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }
        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }
    }
}