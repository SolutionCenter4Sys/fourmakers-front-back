using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Endereco;
using System;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DataTransferObject.Domain
{
    public class ColaboradorBIDTO
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }
        [JsonPropertyName("sobre")]
        public string Sobre { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        private string _contatoPrincipal { get; set; }

        [JsonPropertyName("contatoPrincipal")]
        public string ContatoPrincipal
        {
            get
            {
                return !string.IsNullOrEmpty(_contatoPrincipal) ? Regex.Replace(_contatoPrincipal, "[^0-9]", string.Empty) : null;
            }

            set
            {
                _contatoPrincipal = !string.IsNullOrEmpty(value) ? Regex.Replace(value, "[^0-9]", string.Empty) : null;
            }
        }
        public string ContatoPrincipalDDI { get; set; }

        [JsonPropertyName("contatoOutros")]
        public string ContatoOutros { get; set; }

        [JsonPropertyName("dataNascimento")]
        public DateTime? DataNascimento { get; set; }

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

        [JsonPropertyName("pessoa_refugiada")]
        public bool PessoaRefugiada { get; set; }

        [JsonPropertyName("email_alternativo")]
        public string EmailAlternativo { get; set; }

        [JsonPropertyName("saude")]
        public ColaboradorSaudeDTO Saude { get; set; }

        [JsonPropertyName("nacionaliade")]
        public string Nacionalidade { get; set; }

        [JsonPropertyName("documentoColaborador")]
        public string DocumentoColaborador { get; set; }
    }
}