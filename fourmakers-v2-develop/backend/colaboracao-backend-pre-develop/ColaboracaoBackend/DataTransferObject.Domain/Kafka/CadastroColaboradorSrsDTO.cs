using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Endereco;
using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Kafka
{
    public class CadastroColaboradorSrsDTO
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }

        [JsonPropertyName("dataNascimento")]
        public DateTime? DataNascimento { get; set; }

        [JsonPropertyName("contatoPrincipal")]
        public string ContatoPrincipal { get; set; }

        [JsonPropertyName("phone_home")]
        public string phone_home { get; set; }

        [JsonPropertyName("rg")]
        public string Rg { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("email1")]
        public string Email1 { get; set; }

        [JsonPropertyName("estadoCivil")]
        public string EstadoCivil { get; set; }

        [JsonPropertyName("genero")]
        public string Genero { get; set; }

        [JsonPropertyName("etnia")]
        public string Etnia { get; set; }

        [JsonPropertyName("orientacao_sexual")]
        public string orientacao_sexual { get; set; }

        [JsonPropertyName("pessoa_refugiada")]
        public string pessoa_refugiada { get; set; }

        [JsonPropertyName("nivel_escolaridade")]
        public string nivel_escolaridade { get; set; }

        [JsonPropertyName("diretoria")]
        public DiretoriaDTO Diretoria { get; set; }

        [JsonPropertyName("cargo")]
        public CargoDTO Cargo { get; set; }

        [JsonPropertyName("endereco")]
        public EnderecoDTO Endereco { get; set; }
    }
}