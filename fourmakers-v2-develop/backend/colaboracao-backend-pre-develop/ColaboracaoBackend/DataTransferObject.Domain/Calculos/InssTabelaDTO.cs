using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Calculos
{
    public class InssTabelaDTO
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

        [JsonPropertyName("faixa1ParcelaDeduzir")]
        public decimal Faixa1ParcelaDeduzir { get; set; }

        [JsonPropertyName("faixa2Min")]
        public decimal Faixa2Min { get; set; }

        [JsonPropertyName("faixa2Max")]
        public decimal Faixa2Max { get; set; }

        [JsonPropertyName("faixa2Aliquota")]
        public decimal Faixa2Aliquota { get; set; }

        [JsonPropertyName("faixa2ParcelaDeduzir")]
        public decimal Faixa2ParcelaDeduzir { get; set; }

        [JsonPropertyName("faixa3Min")]
        public decimal Faixa3Min { get; set; }

        [JsonPropertyName("faixa3Max")]
        public decimal Faixa3Max { get; set; }

        [JsonPropertyName("faixa3Aliquota")]
        public decimal Faixa3Aliquota { get; set; }

        [JsonPropertyName("faixa3ParcelaDeduzir")]
        public decimal Faixa3ParcelaDeduzir { get; set; }

        [JsonPropertyName("faixa4Min")]
        public decimal Faixa4Min { get; set; }

        [JsonPropertyName("faixa4Max")]
        public decimal Faixa4Max { get; set; }

        [JsonPropertyName("faixa4Aliquota")]
        public decimal Faixa4Aliquota { get; set; }

        [JsonPropertyName("faixa4ParcelaDeduzir")]
        public decimal Faixa4ParcelaDeduzir { get; set; }

        [JsonPropertyName("tetoInss")]
        public decimal TetoInss { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime DataAlteracao { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
    }
}
