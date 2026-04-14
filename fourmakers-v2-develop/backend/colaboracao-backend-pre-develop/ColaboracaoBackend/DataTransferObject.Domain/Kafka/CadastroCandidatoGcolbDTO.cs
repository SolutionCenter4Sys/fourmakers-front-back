using DataTransferObject.Domain.Endereco;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Kafka
{
    public class CadastroCandidatoGcolbDTO
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }

        [JsonPropertyName("dataNascimento")]
        public DateTime? DataNascimento { get; set; }

        [JsonPropertyName("contatoPrincipal")]
        public string ContatoPrincipal { get; set; }

        [JsonPropertyName("rg")]
        public string Rg { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("endereco")]
        public EnderecoDTO Endereco { get; set; }

        [JsonPropertyName("estadoCivil")]
        public string EstadoCivil { get; set; }

        [JsonPropertyName("etnia")]
        public string Etnia { get; set; }

        [JsonPropertyName("genero")]
        public string Genero { get; set; }

        [JsonPropertyName("orientacaoSexual")]
        public string OrientacaoSexual { get; set; }

        [JsonPropertyName("escolaridade")]
        public string Escolaridade { get; set; }
    }
}