using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao
{
    public class PeriodoDTO
    {
        private DateTime _dataInicio { get; set; }
        private DateTime _dataFim { get; set; }

        [JsonIgnore]
        public long PeriodoAlocadoId { get; set; }
        [JsonIgnore]
        public long ColaboradorAlocadoId { get; set; }
        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio
        {
            get { return new DateTime(_dataInicio.Year, _dataInicio.Month, _dataInicio.Day, 0, 0, 0); }
            set { _dataInicio = value; }
        }
        [JsonPropertyName("dataFim")]
        public DateTime DataFim
        {
            get { return new DateTime(_dataFim.Year, _dataFim.Month, _dataFim.Day, 23, 59, 59); }
            set { _dataFim = value; }
        }
        [JsonPropertyName("dataAlteracao")]
        public DateTime DataAlteracao { get; set; }

        [JsonPropertyName("incluiFimDeSemana")]
        public bool IncluiFimDeSemana { get; set; }
        [JsonPropertyName("quantidadeHoras")]
        public double QuantidadeHoras { get; set; }
        public string Oportunidade { get; set; }
        public string Observacao { get; set; }
        public sbyte Prioritario { get; set; }
        public double? Percentual { get; set; }
    }
}