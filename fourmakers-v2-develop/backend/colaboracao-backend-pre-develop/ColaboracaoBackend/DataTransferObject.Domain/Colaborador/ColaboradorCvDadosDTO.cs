using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Endereco;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorCvDadosDTO
    {
        [JsonPropertyName("cpf")]
        public string Cpf { get; set; }

        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }

        public string Sobre { get; set; }

        public List<VistoColaboradorDTO> Vistos { get; set; }

        public List<PassaporteColaboradorDTO> Passaportes { get; set; }

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

        [JsonPropertyName("slack_id")]
        public string Slack_id { get; set; }

        [JsonPropertyName("dataNascimento")]
        public DateTime? DataNascimento { get; set; }

        [JsonPropertyName("diretoria")]
        public DiretoriaDTO Diretoria { get; set; }

        [JsonPropertyName("status")]
        public StatusColaboradorResult Status { get; set; }

        [JsonPropertyName("endereco")]
        public EnderecoDTO Endereco { get; set; }

        [JsonPropertyName("urlFoto")]
        public string UrlFoto { get; set; }

        [JsonPropertyName("urlFotoThumb")]
        public string UrlFotoThumb { get; set; }

        [JsonPropertyName("urlFotoThumbMini")]
        public string UrlFotoThumbMini { get; set; }

        [JsonPropertyName("urlFotoThumbVeryMini")]
        public string UrlFotoThumbVeryMini { get; set; }

        [JsonPropertyName("escolaridade")]
        public string Escolaridade { get; set; }
    }
}