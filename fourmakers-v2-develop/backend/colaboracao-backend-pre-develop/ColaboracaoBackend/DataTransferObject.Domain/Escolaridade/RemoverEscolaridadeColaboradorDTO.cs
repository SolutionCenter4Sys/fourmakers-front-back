using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Escolaridade
{
    public class RemoverEscolaridadeColaboradorDTO
    {
        [JsonPropertyName("escolaridadeId")]
        public long EscolaridadeId { get; set; }
    }
}