using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Empresa
{
    public class UsuarioEmpresaDTO
    {
        [JsonPropertyName("nome_completo")]
        public string NomeCompleto { get; set; }
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("acesso_permitido")]
        public bool AcessoPermitido { get; set; }
        [JsonPropertyName("data_convite")]
        public DateTime DataConvite { get; set; }
    }
}