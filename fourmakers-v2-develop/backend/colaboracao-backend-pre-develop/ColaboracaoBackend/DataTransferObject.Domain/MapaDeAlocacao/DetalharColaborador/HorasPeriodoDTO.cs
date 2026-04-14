using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador
{
    public class HorasPeriodoDTO
    {
        [JsonPropertyName("periodoId")]
        public long PeriodoId { get; set; }
        [JsonPropertyName("dia")]
        public string DiaPeriodo { get; set; }
        [JsonPropertyName("horasAlocadas")]
        public double? HorasPeriodo { get; set; }
        [JsonPropertyName("ehFinalDeSemana")]
        public bool isFinalDeSemana { get; set; }
        [JsonPropertyName("data")]
        public DateTime Data { get; set; }
    }
}