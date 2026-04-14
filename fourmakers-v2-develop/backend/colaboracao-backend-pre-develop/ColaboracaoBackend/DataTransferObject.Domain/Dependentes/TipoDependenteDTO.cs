using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dependentes
{
    public class TipoDependenteDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonIgnore]
        public sbyte Ativo { get; set; }
    }
}