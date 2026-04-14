using DataTransferObject.Domain.Dependentes;
using System;
using System.Text.Json.Serialization;

namespace Colaborador.API.DTOs
{
    public class AlteraDadosDependenteColaborador
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

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

        [JsonPropertyName("colaboradorCpf")]
        public string ColaboradorCpf { get; set; }

        [JsonPropertyName("ativo")]
        public sbyte Ativo { get; set; }

        [JsonPropertyName("tipoDependente")]
        public TipoDependenteDTO TipoDependente { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime DataAlteracao { get; set; }
    }
}