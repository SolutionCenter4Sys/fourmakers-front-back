using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Empresa
{
    public class UsuarioEmpresaPorIdDTO
    {
        [JsonPropertyName("usuarioId")]
        public long UsuarioId { get; set; }
        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }
        [JsonPropertyName("permiteAcesso")]
        public bool PermiteAcessoDados { get; set; }
    }
}