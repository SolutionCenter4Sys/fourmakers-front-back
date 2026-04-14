using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Empresa
{
    public class UsuarioCpfIdDTO
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }
        [JsonPropertyName("id")]
        public long UsuarioId { get; set; }
    }
}