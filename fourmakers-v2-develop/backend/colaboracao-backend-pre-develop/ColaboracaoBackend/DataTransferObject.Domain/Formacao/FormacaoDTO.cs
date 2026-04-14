using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Formacao
{
    public class FormacaoDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }
    }
}