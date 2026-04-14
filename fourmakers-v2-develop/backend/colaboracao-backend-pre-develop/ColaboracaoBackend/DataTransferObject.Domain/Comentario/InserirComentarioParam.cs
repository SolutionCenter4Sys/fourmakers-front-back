using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Comentario
{
    public class InserirComentarioDTO
    {
        [JsonPropertyName("cpfColaborador")]
        public string CpfColaborador { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("texto")]
        public string Texto { get; set; }
    }
}