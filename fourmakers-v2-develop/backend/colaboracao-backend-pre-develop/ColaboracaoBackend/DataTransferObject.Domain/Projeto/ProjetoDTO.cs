using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Projeto
{
    public class ProjetoDTO
    {
        public ProjetoDTO()
        {
        }

        public ProjetoDTO(long id, string nome, DateTime dataInicio, DateTime dataFim, long idEmpresa)
        {
            Id = id;
            Nome = nome;
            DataInicio = dataInicio;
            DataFim = dataFim;
            EmpresaId = idEmpresa;
        }

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
    }
}