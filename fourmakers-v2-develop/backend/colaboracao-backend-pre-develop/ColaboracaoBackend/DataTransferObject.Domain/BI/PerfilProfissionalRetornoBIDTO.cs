using DataTransferObject.Domain.Competencia;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.BI
{
    public class PerfilProfissionalRetornoBIDTO
    {
        [JsonPropertyName("competencias")]
        public List<CompetenciaColaboradorDTO> Competencias { get; set; }
    }
}