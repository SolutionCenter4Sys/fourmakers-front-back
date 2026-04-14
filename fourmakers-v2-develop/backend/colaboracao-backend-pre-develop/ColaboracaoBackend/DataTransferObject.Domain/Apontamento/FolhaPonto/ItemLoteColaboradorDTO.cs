using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento.FolhaPonto
{
    public class ItemLoteColaboradorDTO : ItemLoteDTO
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("cargo")]
        public string Cargo { get; set; }

        [JsonPropertyName("matricula")]
        public string Matricula { get; set; }
        
        [JsonPropertyName("erro")]
        public string Erro { get; set; }

    }
} 