using DataTransferObject.Domain.Base;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario
{
    public class UsuarioValidacaoResult : StatusResult
    {
        [JsonPropertyName("usuario")]
        public UsuarioColaboradorDTO Usuario { get; set; }
    }
}