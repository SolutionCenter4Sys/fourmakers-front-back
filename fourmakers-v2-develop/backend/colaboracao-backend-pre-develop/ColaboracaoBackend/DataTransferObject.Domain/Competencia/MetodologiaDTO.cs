using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Competencia
{
    public class MetodologiaDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
        [JsonPropertyName("gestorExternoPerfil")]
        public string GestorExternoPerfil { get; set; }

        [JsonIgnore]
        [JsonPropertyName("usuarioCriacaoId")]
        public long UsuarioCriacaoId { get; set; }

        [JsonPropertyName("pendente")]
        public bool Pendente { get; set; }

        [JsonPropertyName("nivelId")]
        public int NivelId { get; set; }
        [JsonIgnore]
        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
    }
}