using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario
{
    public class IdIndicacaoUsuario
    {
        [JsonPropertyName("idIndicacao")]
        public long IdIndicacao { get; set; }

        [JsonPropertyName("usuario")]
        public UsuarioColaboradorDTO Usuario { get; set; }
    }
}