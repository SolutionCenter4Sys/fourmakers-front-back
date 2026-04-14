using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario
{
    public class ConviteUsuarioExternoDTO
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; }
    }
}