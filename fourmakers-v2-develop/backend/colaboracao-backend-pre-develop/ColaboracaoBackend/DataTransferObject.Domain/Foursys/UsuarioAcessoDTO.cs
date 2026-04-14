using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Foursys
{
    public class UsuarioAcessoDTO
    {
        [JsonPropertyName("id")]
        public long? Id { get; set; }
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }
        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("diretoria")]
        public string Diretoria { get; set; }
        [JsonPropertyName("imagemPath")]
        public string ImagemPath { get; set; }

        [JsonPropertyName("ultimo_login")]
        public DateTime? UltimoLogin { get; set; }
    }
}