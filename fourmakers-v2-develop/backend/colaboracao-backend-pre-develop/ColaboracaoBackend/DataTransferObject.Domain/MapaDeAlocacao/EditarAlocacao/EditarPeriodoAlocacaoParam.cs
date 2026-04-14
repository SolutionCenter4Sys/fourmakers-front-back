using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.MapaDeAlocacao.EditarAlocacao
{
    public class EditarPeriodoAlocacaoParam
    {
        [JsonPropertyName("periodoAlocacaoId")]
        public long PeriodoAlocacaoId { get; set; }
        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }
        [JsonPropertyName("dataFim")]
        public DateTime DataFim { get; set; }
        [JsonPropertyName("quantidadeDeHoras")]
        public double QuantidadeDeHoras { get; set; }
        [JsonPropertyName("percentual")]
        public double? Percentual { get; set; }
        [JsonPropertyName("oportunidade")]
        public string Oportunidade { get; set; }
        [JsonPropertyName("observacao")]
        public string Observacao { get; set; }
        [JsonPropertyName("prioritario")]
        public sbyte? Prioritario { get; set; }
        [JsonPropertyName("incluiFimDeSemana")]
        public bool? IncluiFimDeSemana { get; set; }
        public bool? FlagRetroalimentaCV { get; set; }
        public string IdPerfilAlocacao { get; set; }
        public List<ItemSkillPerfilAlocacaoDTO> PerfilSkills { get; set; }
    }
}