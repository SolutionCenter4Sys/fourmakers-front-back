using System.Collections.Generic;

namespace DataTransferObject.Domain.Competencia
{
    public class CompetenciaSumarioDTO
    {
        public string Skill { get; set; }
        public long CdSkill { get; set; }
        public int QtdUsuarios { get; set; }
        public List<SenioridadeDTO> QtdSenioridade { get; set; }
    }

    public class SenioridadeDTO
    {
        public long CdSkill { get; set; }
        public string Senioridade { get; set; }
        public int QtdUsuarios { get; set; }
    }
}