using DataTransferObject.Domain.Base;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dependentes
{
    public class AdicionarDependentesResult : StatusResult
    {
        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }

        [JsonPropertyName("dataNascimento")]
        public DateTime DataNascimento { get; set; }

        [JsonPropertyName("rg")]
        public string Rg { get; set; }

        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("portadorDeficiencia")]
        public sbyte PortadorDeficiencia { get; set; }

        [JsonPropertyName("requerAjudaQual")]
        public string RequerAjudaQual { get; set; }
    }
}