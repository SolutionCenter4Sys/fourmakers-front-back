using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorBasicoDTO
    {
        [JsonPropertyName("codigoInternoColaborador")]
        public string CodigoInternoColaborador { get; set; }

        [JsonPropertyName("nomeCompleto")]
        public string NomeCompleto { get; set; }

        [JsonPropertyName("dataNascimento")]
        public DateTime? DataNascimento { get; set; }

        [JsonPropertyName("rg")]
        public string Rg { get; set; }

        [JsonPropertyName("matricula")]
        public string Matricula { get; set; }

        [JsonPropertyName("enderecoId")]
        public long? EnderecoId { get; set; }

        [JsonPropertyName("ativo")]
        public bool Ativo { get; set; }

        [JsonPropertyName("dataCriacao")]
        public DateTime DataCriacao { get; set; }

        [JsonPropertyName("dataAlteracao")]
        public DateTime DataAlteracao { get; set; }

        [JsonPropertyName("contatoPrincipalDDI")]
        public string ContatoPrincipalDDI { get; set; }

        [JsonPropertyName("contatoPrincipal")]
        public string ContatoPrincipal { get; set; }

        [JsonPropertyName("contatoOutro")]
        public string ContatoOutro { get; set; }

        [JsonPropertyName("imagemId")]
        public long? ImagemId { get; set; }

        [JsonPropertyName("candidato")]
        public bool Candidato { get; set; }

        [JsonPropertyName("passaporte")]
        public string Passaporte { get; set; }

        [JsonPropertyName("colaboradorSaudeId")]
        public int? ColaboradorSaudeId { get; set; }

        [JsonPropertyName("estadoCivil")]
        public string EstadoCivil { get; set; }

        [JsonPropertyName("genero")]
        public string Genero { get; set; }

        [JsonPropertyName("etnia")]
        public string Etnia { get; set; }

        [JsonPropertyName("orientacaoSexual")]
        public string OrientacaoSexual { get; set; }

        [JsonPropertyName("escolaridade")]
        public string Escolaridade { get; set; }

        [JsonPropertyName("refugiado")]
        public bool? Refugiado { get; set; }

        [JsonPropertyName("emailAlternativo")]
        public string EmailAlternativo { get; set; }

        [JsonPropertyName("nacionalidade")]
        public string Nacionalidade { get; set; }

        [JsonPropertyName("documentoColaborador")]
        public string DocumentoColaborador { get; set; }

        [JsonPropertyName("urlLinkedin")]
        public string UrlLinkedin { get; set; }

        [JsonPropertyName("dataSyncLinkedin")]
        public DateTime? DataSyncLinkedin { get; set; }

        [JsonPropertyName("visualizarBuscaAderencia")]
        public bool VisualizarBuscaAderencia { get; set; }

        [JsonPropertyName("qualificado")]
        public bool? Qualificado { get; set; }
    }
} 