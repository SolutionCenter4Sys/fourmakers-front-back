using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Usuario
{
    public class ConfirmacaoEmailDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nomeColaborador")]
        public string NomeColaborador { get; set; }

        [JsonPropertyName("usuarioId")]
        public long UsuarioId { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("codigo_enviado")]
        public int CodigoEnviado { get; set; }

        [JsonPropertyName("codigo_confirmado")]
        public bool CodigoConfirmado { get; set; }

        [JsonPropertyName("hora_expiracao_codigo")]
        public DateTime HoraExpiracaoCodigo { get; set; }
    }
}