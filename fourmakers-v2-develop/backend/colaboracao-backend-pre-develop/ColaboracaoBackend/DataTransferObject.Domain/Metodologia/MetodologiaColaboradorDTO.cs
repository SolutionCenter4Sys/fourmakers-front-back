using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Nivel;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Metodologia
{
    public class MetodologiaColaboradorDTO
    {
        [JsonPropertyName("metodologia")]
        public MetodologiaDTO Metodologia { get; set; }

        [JsonPropertyName("perfilMetodologia")]
        public ItemPerfilDTO PerfilMetodologia { get; set; }

        [JsonPropertyName("nivel")]
        public NivelDTO Nivel { get; set; }
    }
}