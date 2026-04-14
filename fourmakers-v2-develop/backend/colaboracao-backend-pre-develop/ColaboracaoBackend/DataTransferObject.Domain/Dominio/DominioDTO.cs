using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dominio
{
    public class DominioDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("usuarioCriacaoId")]
        public long UsuarioCriacaoId { get; set; }

        [JsonPropertyName("cpfUsuarioCriacao")]
        public string CpfUsuarioCriacao { get; set; }

        [JsonPropertyName("pendente")]
        public bool Pendente { get; set; }
    }
}