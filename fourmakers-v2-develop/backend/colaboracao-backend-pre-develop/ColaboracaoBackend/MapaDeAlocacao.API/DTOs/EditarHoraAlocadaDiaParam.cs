using System;
using System.Text.Json.Serialization;

namespace MapaDeAlocacao.API.DTOs
{
    public class EditarHoraAlocadaDiaParam
    {
        [JsonPropertyName("colaboradorAlocadoId")]
        public long ColaboradorAlocadoId { get; set; }
        [JsonPropertyName("periodoAlocacaoId")]
        public long PeriodoAlocacaoId { get; set; }
        [JsonPropertyName("diaASerEditado")]
        public DateTime DiaASerEditado { get; set; }
        [JsonPropertyName("quantidadeDeHoras")]
        public double QuantidadeDeHoras { get; set; }
        public double Percentual { get; set; }
        public string Oportunidade { get; set; }
        public string Observacao { get; set; }
        public sbyte Prioritario { get; set; }
    }
}