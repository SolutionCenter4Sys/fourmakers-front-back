using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Nivel;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Metodologia
{
    public class ListaMetodologiaColaboradorResult
    {
        [JsonPropertyName("metodologia")]
        public MetodologiaDTO Metodologia { get; set; }

        [JsonPropertyName("nivel")]
        public NivelDTO Nivel { get; set; }
    }
}