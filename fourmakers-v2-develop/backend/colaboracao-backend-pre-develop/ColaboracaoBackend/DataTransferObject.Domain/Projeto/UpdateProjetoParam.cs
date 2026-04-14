using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class UpdateProjetoParam
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("dataInicio")]
        public DateTime DataInicio { get; set; }

        [JsonPropertyName("dataFim")]
        public DateTime DataFim { get; set; }

        [JsonPropertyName("empresaId")]
        public long EmpresaId { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }
    }
}