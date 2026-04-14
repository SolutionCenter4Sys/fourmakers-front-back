using System;
using DataTransferObject.Domain.Competencia;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class InserirVagaRecrutamentoDTO
    {
        public string Titulo { get; set; }
        public int? NumeroDeVagas { get; set; }
        public decimal CustoProfissional { get; set; }
        public decimal RateCard { get; set; }
        public string Descricao { get; set; }
        public string Cargo { get; set; }
        public string Localizacao { get; set; }
        public string Estado { get; set; }
        public string Cidade { get; set; }
        public string Cep { get; set; }
        public string Pais { get; set; }
        public string CodigoGestor { get; set; }
        public string StatusVagaCod { get; set; }
        public string OrgId { get; set; }
        public List<SkillNivelDTO> Hardskills { get; set; }
        public List<SkillNivelDTO> Softskills { get; set; }
        public List<SkillNivelDTO> Metodologias { get; set; }
        public List<SkillNivelDTO> Dominios { get; set; }
        public List<SkillNivelDTO> Idiomas { get; set; }
        public string IdPerfilGerador { get; set; }
        public string Frequencia { get; set; }
        public int? ModeloTrabalhoCod { get; set; }
        public Guid? TipoEmpregoLinkedin { get; set; }
        public Guid? NivelExperienciaLinkedin { get; set; }
        public Guid? ModeloTrabalhoId { get; set; }
        public Guid? PermanenciaId { get; set; }
    }

    // DTO para envio da vaga para o LinkedIn via Azure Logic Apps
    public class VagaLinkedinRequestDTO
    {
        public string titulo { get; set; }
        public string cargo { get; set; }
        public int localTrabalho { get; set; }
        public string localidade { get; set; }
        public int tipoEmprego { get; set; }
        public int senioridade { get; set; }
        public string descricaoVaga { get; set; }
        public List<string> competencias { get; set; }
        public bool requererCurriculo { get; set; }
        public bool triagemEliminatoria { get; set; }
        public bool arquivarForaDoPais { get; set; }
        public string codigoVaga { get; set; }
        public string urlVaga { get; set; }

        public VagaLinkedinRequestDTO()
        {
            competencias = new List<string>();
        }
    }
} 