using System;

namespace DataTransferObject.Domain.Vaga
{
    public class OrganizacaoCandidatoDTO
    {
        public int OrgId { get; set; }
        public string OrgDescricao { get; set; }
        public bool? AtivoNaOrg { get; set; }
        public string TipoCadastroBancoDeTalentos { get; set; }
        public int OrgIdTbBancoTalento { get; set; }
        public int OrgIdTbColaboradorOrg { get; set; }
        public string CodDiretoria { get; set; }
    }
} 