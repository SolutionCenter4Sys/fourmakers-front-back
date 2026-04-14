using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Match;

namespace DataTransferObject.Domain.Vaga
{
    public class ListarCandidatosAderentesResult
    {
        public string Nome { get; set; }
        public string Codigo { get; set; }
        public double PercentualAderencia { get; set; }
        public string Email { get; set; }
        public bool EhCandidato { get; set; }
        public CandidatosMatchResponse RetornoMatch { get; set; }
        public bool? Qualificado { get; set; }
        public int OrgId { get; set; }
        public string OrgDescricao { get; set; }
        public bool? AtivoNaOrg { get; set; }
        public string Origem { get; set; }
        public string Comunidade { get; set; }
        public List<OrganizacaoCandidatoDTO> Organizacoes { get; set; } = new List<OrganizacaoCandidatoDTO>();
    }
}