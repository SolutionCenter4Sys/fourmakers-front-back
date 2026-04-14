using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Endosso
{
    public class TipoEndossoDTO
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; }

        [JsonPropertyName("nivelEndosso")]
        public int NivelEndosso { get; set; }
    }
}