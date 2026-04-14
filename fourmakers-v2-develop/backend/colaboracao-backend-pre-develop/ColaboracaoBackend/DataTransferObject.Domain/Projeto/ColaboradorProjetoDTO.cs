using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class ColaboradorProjetoDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("valorDefinido")]
        public decimal ValorDefinido { get; set; }

        [JsonPropertyName("ativo")]
        public sbyte Ativo { get; set; }

        [JsonPropertyName("modalidadeId")]
        public long ModalidadeId { get; set; }

        [JsonPropertyName("projetoId")]
        public long ProjetoId { get; set; }

        [JsonPropertyName("colaboradorCpf")]
        public string ColaboradorCpf { get; set; }

        [JsonPropertyName("dataInicio")]
        public DateTime dataInicio { get; set; }

        [JsonPropertyName("dataFinal")]
        public DateTime dataFinal { get; set; }
    }
}