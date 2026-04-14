using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento
{
    public class ProjetoDTO
    {
        [JsonPropertyName("id")]
        public string CodProjeto { get; set; }
        public string NomeProjeto { get; set; }
        public string Gerente { get; set; }
        public string CodCliente { get; set; }
        public string NomeCliente { get; set; }
        public bool PermiteApontamentoSemAlocacao { get; set; }
        public bool PermiteApontamentoSemAlocacaoParaOutroColaborador { get; set; }
        public DateTime? DataFim { get; set; }
    }
}