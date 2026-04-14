using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Firebase.API.DTOs
{
    public class ParamListaFirebase
    {
        [JsonPropertyName("tokens")]
        public List<string> Tokens { get; set; }
        [JsonPropertyName("titulo")]
        public string Titulo { get; set; }
        [JsonPropertyName("mensagem")]
        public string Mensagem { get; set; }
    }
}