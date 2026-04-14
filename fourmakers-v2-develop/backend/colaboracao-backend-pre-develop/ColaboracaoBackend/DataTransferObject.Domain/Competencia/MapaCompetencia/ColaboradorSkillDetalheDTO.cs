using System;

namespace DataTransferObject.Domain.Competencia.MapaCompetencia
{
    public class ColaboradorSkillDetalheDTO
    {
        public string CodColaborador { get; set; }
        public string NomeColaborador { get; set; }
        public string EmailColaborador { get; set; }
        public string CodUnidade { get; set; }
        public string Unidade { get; set; }
        public string CodColaboradorGestor { get; set; }
        public string NomeColaboradorGestor { get; set; }
        public string Hardskills { get; set; }
        public string SoftSkills { get; set; }
        public string Metodologias { get; set; }
        public string DominiosNegocio { get; set; }
        public string Idiomas { get; set; }
        public string Formacoes { get; set; }
        public DateTime Admissao { get; set; }
        public int TempoDeCasaEmDias { get; set; }
        public string CodigoCargo { get; set; }
        public string Cargo { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Clientes { get; set; }
        public int OrgId { get; set; }
        public string Cidadanias { get; set; }
        public string Vistos { get; set; }
        public string Passaportes { get; set; }
    }
}