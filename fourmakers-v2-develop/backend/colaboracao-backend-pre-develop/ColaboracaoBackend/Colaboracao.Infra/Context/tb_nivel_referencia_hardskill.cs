using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_nivel_referencia_hardskill
    {
        public tb_nivel_referencia_hardskill()
        {
            tb_colaborador_referencia_hardskill = new HashSet<tb_colaborador_referencia_hardskill>();
        }

        public int id { get; set; }
        public string descricao { get; set; }

        public virtual ICollection<tb_colaborador_referencia_hardskill> tb_colaborador_referencia_hardskill { get; set; }
    }
}