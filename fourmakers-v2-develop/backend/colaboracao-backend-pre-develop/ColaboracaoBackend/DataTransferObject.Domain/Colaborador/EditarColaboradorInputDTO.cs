using DataTransferObject.Domain.Endereco;
using System;

namespace DataTransferObject.Domain.Colaborador
{
    /// <summary>
    /// DTO de entrada para EditarColaborador que aceita grupoDeRiscoCovid como booleano
    /// </summary>
    public class EditarColaboradorInputDTO
    {
        public string CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Rg { get; set; }
        public string Matricula { get; set; }
        public long? EnderecoId { get; set; }
        public bool Ativo { get; set; }
        public string ContatoPrincipalDdi { get; set; }
        public string ContatoPrincipal { get; set; }
        public string ContatoOutro { get; set; }
        public long? ImagemId { get; set; }
        public bool Candidato { get; set; }
        public string Passaporte { get; set; }
        public int? ColaboradorSaudeId { get; set; }
        public string EstadoCivil { get; set; }
        public string Genero { get; set; }
        public string Etnia { get; set; }
        public string OrientacaoSexual { get; set; }
        public string Escolaridade { get; set; }
        public bool? Refugiado { get; set; }
        public string EmailAlternativo { get; set; }
        public string Nacionalidade { get; set; }
        public string Sobre { get; set; }
        public string DocumentoColaborador { get; set; }
        public string UrlLinkedin { get; set; }
        public DateTime? DataSyncLinkedin { get; set; }
        public bool VisualizarBuscaAderencia { get; set; }
        public bool? Qualificado { get; set; }
        public EnderecoDTO? Endereco { get; set; }
        public ColaboradorSaudeInputDTO? Saude { get; set; }
    }
}

