using DataTransferObject.Domain.Nivel;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dominio
{
    public class DominioNivelDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("nivel")]
        public NivelDTO Nivel { get; set; }
    }
}