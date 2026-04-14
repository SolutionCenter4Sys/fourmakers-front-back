using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.DetalharColaborador
{
    public class PeriodoAlocadoColaboradorDTO
    {
        [JsonPropertyName("periodoAlocadoId")]
        public long PeriodoAlocadoId { get; set; }
        [JsonPropertyName("colaboradorAlocadoId")]
        public long ColaboradorAlocadoId { get; set; }
        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }
        [JsonPropertyName("dataFim")]
        public DateTime DataFim { get; set; }
        [JsonPropertyName("horasPorDia")]
        public double HorasPorDia { get; set; }
        [JsonPropertyName("dataAlteracao")]
        public DateTime DataAlteracao { get; set; }
        [JsonPropertyName("incluiFimDeSemana")]
        public bool IncluiFimDeSemana { get; set; }
        [JsonPropertyName("observacao")]
        public string Observacao { get; set; }
        [JsonPropertyName("oportunidade")]
        public string Oportunidade { get; set; }
        [JsonPropertyName("prioritario")]
        public sbyte Prioritario { get; set; }
        [JsonPropertyName("percentual")]
        public double? Percentual { get; set; }
    }
}