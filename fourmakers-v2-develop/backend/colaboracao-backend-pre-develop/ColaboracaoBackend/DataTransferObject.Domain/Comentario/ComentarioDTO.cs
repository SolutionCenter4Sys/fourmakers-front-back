using DataTransferObject.Domain.Colaborador;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Comentario
{
    public class ComentarioDTO
    {
        public ComentarioDTO()
        {
            Colaborador = new ColaboradorDTO();
        }

        [JsonPropertyName("comentario_tipo")]
        public int IdTipoComentario { get; set; }

        [JsonPropertyName("id")]
        public long IdComentario { get; set; }

        [JsonIgnore]
        public string ColaboradorCpf { get; set; }

        [JsonPropertyName("texto")]
        public string Texto { get; set; }

        [JsonPropertyName("dataComentario")]
        public string DataComentario { get; set; }

        [JsonPropertyName("colaborador")]
        public ColaboradorDTO Colaborador { get; set; }

        [JsonPropertyName("tipoComentario")]
        public ComentarioTipoDTO ComentarioTipo { get; set; }

        [JsonIgnore]
        public int totalResultCount { get; set; }

        [JsonIgnore]
        public int filteredResultCount { get; set; }
    }
}