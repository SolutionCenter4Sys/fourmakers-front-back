using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador
{
    public class DisponibilidadeHoras
    {
        [JsonPropertyName("dia")]
        public string DiaDisponibilidade { get; set; }
        [JsonPropertyName("Horas")]
        public double HorasDisponiveis { get; set; }
        [JsonPropertyName("ehFinalDeSemana")]
        public bool EhFinalDeSemana { get; set; }
        [JsonPropertyName("data")]
        public DateTime Data { get; set; }
    }
}