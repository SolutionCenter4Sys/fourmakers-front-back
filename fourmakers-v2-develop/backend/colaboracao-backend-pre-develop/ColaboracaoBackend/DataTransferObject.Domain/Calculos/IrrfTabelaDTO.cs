using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Calculos
{
    public class IrrfTabelaDTO
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("ano")]
        public int Ano { get; set; }

        [JsonPropertyName("faixa1Min")]
        public decimal Faixa1Min { get; set; }

        [JsonPropertyName("faixa1Max")]
        public decimal Faixa1Max { get; set; }

        [JsonPropertyName("faixa1Aliquota")]
        public decimal Faixa1Aliquota { get; set; }

        [JsonPropertyName("faixa1Deducao")]
        public decimal Faixa1Deducao { get; set; }

        [JsonPropertyName("faixa2Min")]
        public decimal Faixa2Min { get; set; }

        [JsonPropertyName("faixa2Max")]
        public decimal Faixa2Max { get; set; }

        [JsonPropertyName("faixa2Aliquota")]
        public decimal Faixa2Aliquota { get; set; }

        [JsonPropertyName("faixa2Deducao")]
        public decimal Faixa2Deducao { get; set; }

        [JsonPropertyName("faixa3Min")]
        public decimal Faixa3Min { get; set; }

        [JsonPropertyName("faixa3Max")]
        public decimal Faixa3Max { get; set; }

        [JsonPropertyName("faixa3Aliquota")]
        public decimal Faixa3Aliquota { get; set; }

        [JsonPropertyName("faixa3Deducao")]
        public decimal Faixa3Deducao { get; set; }

        [JsonPropertyName("faixa4Min")]
        public decimal Faixa4Min { get; set; }

        [JsonPropertyName("faixa4Max")]
        public decimal Faixa4Max { get; set; }

        [JsonPropertyName("faixa4Aliquota")]
        public decimal Faixa4Aliquota { get; set; }

        [JsonPropertyName("faixa4Deducao")]
        public decimal Faixa4Deducao { get; set; }

        [JsonPropertyName("faixa5Min")]
        public decimal Faixa5Min { get; set; }

        [JsonPropertyName("faixa5Max")]
        public decimal Faixa5Max { get; set; }

        [JsonPropertyName("faixa5Aliquota")]
        public decimal Faixa5Aliquota { get; set; }

        [JsonPropertyName("faixa5Deducao")]
        public decimal Faixa5Deducao { get; set; }

        [JsonPropertyName("deducaoPorDependente")]
        public decimal DeducaoPorDependente { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime DataAlteracao { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
    }
}
