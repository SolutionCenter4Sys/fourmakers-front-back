using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Competencia
{
    public class FiltroSumarioCompetenciasParam
    {
        [JsonPropertyName("unidadeid")]
        public List<long> UnidadeId { get; set; }
        public List<string> perfilId { get; set; }
        public List<FiltroCompetenciaDTO> competencia { get; set; }
        public List<string> Cpf { get; set; }
        public int Limite { get; set; }
        public int Cursor { get; set; }
    }
}