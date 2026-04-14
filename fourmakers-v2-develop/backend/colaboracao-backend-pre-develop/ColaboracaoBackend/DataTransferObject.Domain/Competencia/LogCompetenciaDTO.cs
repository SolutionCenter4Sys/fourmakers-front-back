using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Competencia
{
    public class LogCompetenciaDTO
    {
        [JsonPropertyName("descricaoCompetencia")]
        public string DescricaoCompetencia { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime DataAlteracao { get; set; }

        [JsonPropertyName("observacao")]
        public string Observacao { get; set; }

        [JsonPropertyName("situacao")]
        public string Situacao { get; set; }
    }
}