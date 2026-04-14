using System.Text.Json.Serialization;

namespace Usuario.API.DTOs
{
    public class InsereUsuarioFourmakerParam
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("nome_completo")]
        public string NomeCompleto { get; set; }
        [JsonPropertyName("senha")]
        public string Senha { get; set; }
    }
}