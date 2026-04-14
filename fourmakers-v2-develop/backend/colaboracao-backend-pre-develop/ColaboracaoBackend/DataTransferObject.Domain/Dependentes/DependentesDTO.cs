using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dependentes
{
    public class DependentesDTO
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

        [JsonIgnore]
        public sbyte Ativo { get; set; }

        [JsonPropertyName("tipoDependente")]
        public TipoDependenteDTO TipoDependente { get; set; }

        [JsonIgnore]
        public int tipoDependenteId { get; set; }

        [JsonIgnore]
        public DateTime DataCriacao { get; set; }

        [JsonIgnore]
        public DateTime DataAlteracao { get; set; }

        [JsonIgnore]
        public long UsuarioCriacaoId { get; set; }
    }
}