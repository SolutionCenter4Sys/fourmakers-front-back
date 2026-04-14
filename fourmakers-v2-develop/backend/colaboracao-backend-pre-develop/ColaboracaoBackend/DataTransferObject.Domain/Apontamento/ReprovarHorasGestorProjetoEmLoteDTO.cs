using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento
{
    public class ReprovarHorasGestorProjetoEmLoteDTO
    {
        [JsonPropertyName("ids")]
        public List<string> Ids { get; set; }
        [JsonPropertyName("justificativa")]
        public string Justificativa { get; set; }
    }
}