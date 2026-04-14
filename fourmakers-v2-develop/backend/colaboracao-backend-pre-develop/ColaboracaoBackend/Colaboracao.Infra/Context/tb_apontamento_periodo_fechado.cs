using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_apontamento_periodo_fechado
    {
        public tb_apontamento_periodo_fechado()
        {
            tb_apontamento_periodo_fechado_log = new HashSet<tb_apontamento_periodo_fechado_log>();
        }

        public int id { get; set; }
        public DateTime? data_fim { get; set; }
        public int? tb_org_id { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }
        public string codigo_interno_colaborador_criacao { get; set; }
        public string codigo_interno_colaborador_alteracao { get; set; }

        public virtual tb_org tb_org { get; set; }
        public virtual ICollection<tb_apontamento_periodo_fechado_log> tb_apontamento_periodo_fechado_log { get; set; }
    }
}