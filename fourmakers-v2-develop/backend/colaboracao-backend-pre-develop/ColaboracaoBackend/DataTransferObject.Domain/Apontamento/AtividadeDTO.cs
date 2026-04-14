using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento
{
    public class AtividadeDTO
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        public string Descricao { get; set; }
    }
}