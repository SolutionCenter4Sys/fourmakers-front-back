using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario
{
    public class UsuarioSistemicoResult : UsuarioResult
    {
        [JsonPropertyName("tokenBubble")]
        public string TokenBubble { get; set; }
    }
}