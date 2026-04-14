using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Dependentes
{
    public class AlterarDependentesDTO
    {
        public AlterarDependentesDTO()
        {
            TipoDependente = new TipoDependenteDTO();
        }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }

        [JsonPropertyName("dataNascimento")]
        public DateTime? DataNascimento { get; set; }

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

        [JsonIgnore]
        public int TipoDependenteId { get; set; }

        [JsonPropertyName("tipoDependente")]
        public TipoDependenteDTO TipoDependente { get; set; }

        [JsonIgnore]
        public DateTime DataAlteracao { get; set; }

        [JsonIgnore]
        public long UsuarioCriacaoId { get; set; }
    }
}