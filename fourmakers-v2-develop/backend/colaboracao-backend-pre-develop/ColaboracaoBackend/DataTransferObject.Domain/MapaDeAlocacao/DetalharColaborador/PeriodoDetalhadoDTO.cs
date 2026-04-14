using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador
{
    public class PeriodoDetalhadoDTO
    {
        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }
        [JsonPropertyName("dataFim")]
        public DateTime DataFim { get; set; }
        [JsonPropertyName("horasPorDia")]
        public double HorasPorDia { get; set; }
        [JsonPropertyName("periodoHoras")]
        public List<HorasPeriodoDTO> HorasPeriodo { get; set; }
    }
}