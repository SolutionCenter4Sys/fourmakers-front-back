using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Domain.Competencia.DashboardMinhaJornada
{
    public class TopDezDashboardMinhaJornada
    {
        public Dictionary<EnumSkillsMovimentacao, List<SkillTopDezDTO>> TopDezPorMovimentacao { get; set; }
        = new();
    }

    public class SkillTopDezDTO
    {
        public int SkillId { get; set; }
        public EnumSkillsMovimentacao Movimentacao { get; set; }
        public string NomeSkill { get; set; } = string.Empty;
        public int Total { get; set; }
    }

}
