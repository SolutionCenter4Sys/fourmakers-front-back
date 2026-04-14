using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.EditarAlocacao
{
    public class EditarHoraDiaDTO
    {
        [JsonPropertyName("orgId")]
        public int OrgId { get; set; }
        [JsonPropertyName("cpfColaborador")]
        public string CpfColaborador { get; set; }
        [JsonPropertyName("codigoProjeto")]
        public long CodigoProjeto { get; set; }
        [JsonPropertyName("idPeriodoAlocacao")]
        public long AlocacaoId { get; set; }
        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }

        [JsonPropertyName("dataFim")]
        public DateTime DataFim { get; set; }

        [JsonPropertyName("incluiFimDeSemana")]
        public bool IncluiFimDeSemana { get; set; }

        [JsonPropertyName("quantidadeHoras")]
        public double QuantidadeHoras { get; set; }
        public double Percentual { get; set; }
        public string Oportunidade { get; set; }
        public string Observacao { get; set; }
        public sbyte Prioritario { get; set; }
    }
}