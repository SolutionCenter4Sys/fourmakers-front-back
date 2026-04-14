using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Dominio;
using DataTransferObject.Domain.Experiencia;
using DataTransferObject.Domain.Formacao;
using DataTransferObject.Domain.Hobby;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Interesse;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Softskill;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Colaborador
{
    public class PerfilProfissionalDTO
    {
        public List<CompetenciaColaboradorDTO> Competencias { get; set; }
        public List<FormacaoColaboradorDTO> Formacoes { get; set; }
        public List<DominioColaboradorDTO> Dominios { get; set; }
        public List<MetodologiaColaboradorDTO> Metodologias { get; set; }
        public List<InteresseColaboradorDTO> Interesses { get; set; }
        public List<HobbyColaboradorDTO> Hobbies { get; set; }
        public List<SoftskillColaboradorDTO> Softskills { get; set; }
        public List<CertificadoColaboradorDTO> Certificados { get; set; }
        public List<IdiomaColaboradorDTO> Idiomas { get; set; }
        public List<ExperienciaEmpresaDTO> ExperienciaEmpresas { get; set; }
    }
}