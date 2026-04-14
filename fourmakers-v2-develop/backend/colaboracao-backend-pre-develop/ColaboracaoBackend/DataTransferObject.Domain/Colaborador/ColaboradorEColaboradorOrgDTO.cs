using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class ColaboradorEColaboradorOrgDTO
    {
        // Campos de tb_colaborador
        public string CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string Rg { get; set; }
        public string Matricula { get; set; }
        public int? EnderecoId { get; set; }
        public int Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string ContatoPrincipalDdi { get; set; }
        public string ContatoPrincipal { get; set; }
        public string ContatoOutro { get; set; }
        public int? ImagemId { get; set; }
        public int Candidato { get; set; }
        public string Passaporte { get; set; }
        public int? ColaboradorSaudeId { get; set; }
        public string EstadoCivil { get; set; }
        public string Genero { get; set; }
        public string Etnia { get; set; }
        public string OrientacaoSexual { get; set; }
        public string Escolaridade { get; set; }
        public int Refugiado { get; set; }
        public string EmailAlternativo { get; set; }
        public string Nacionalidade { get; set; }
        public string DocumentoColaborador { get; set; }
        public string UrlLinkedin { get; set; }
        public DateTime? DataSyncLinkedin { get; set; }
        public int VisualizarBuscaAderencia { get; set; }
        public int? Qualificado { get; set; }

        // Lista de organizações do colaborador
        public List<ColaboradorOrgsDTO> ColaboradorOrgs { get; set; }
    }

    public class ColaboradorOrgsDTO
    {
        public int TbOrgId { get; set; }
        public string CodDiretoria { get; set; }
        public string Diretoria { get; set; }
        public string Departamento { get; set; }
        public string CodDepartamento { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Cargo { get; set; }
        public string CodigoCargo { get; set; }
        public string CodColaboradorExterno { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public int Ativo { get; set; }
        public DateTime? DataInativacao { get; set; }
        public string ModeloContratacao { get; set; }
        public string EmpresaRelacionada { get; set; }
        public string ModeloTrabalho { get; set; }
        public int? DiasPorSemana { get; set; }
        public decimal? ValorHora { get; set; }
        public decimal? CustoHora { get; set; }
        public decimal? BaseHoraMes { get; set; }
        public string Idioma { get; set; }
    }
}