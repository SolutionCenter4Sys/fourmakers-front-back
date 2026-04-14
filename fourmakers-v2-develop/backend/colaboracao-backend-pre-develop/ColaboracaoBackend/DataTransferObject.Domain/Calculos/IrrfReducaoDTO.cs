using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Calculos
{
    public class IrrfReducaoDTO
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("ano")]
        public int Ano { get; set; }

        [JsonPropertyName("faixa1Max")]
        public decimal Faixa1Max { get; set; }

        [JsonPropertyName("faixa1DescontoMaximo")]
        public decimal Faixa1DescontoMaximo { get; set; }

        [JsonPropertyName("faixa2Min")]
        public decimal Faixa2Min { get; set; }

        [JsonPropertyName("faixa2Max")]
        public decimal Faixa2Max { get; set; }

        [JsonPropertyName("faixa2ValorBase")]
        public decimal Faixa2ValorBase { get; set; }

        [JsonPropertyName("faixa2Coeficiente")]
        public decimal Faixa2Coeficiente { get; set; }

        [JsonPropertyName("faixa3Min")]
        public decimal Faixa3Min { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime DataAlteracao { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
    }
}

