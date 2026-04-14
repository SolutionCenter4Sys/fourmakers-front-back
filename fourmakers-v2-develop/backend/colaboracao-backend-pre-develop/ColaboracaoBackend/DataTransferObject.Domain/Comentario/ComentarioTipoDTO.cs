using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Comentario
{
    public class ComentarioTipoDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("texto")]
        public string Descricao { get; set; }
    }
}