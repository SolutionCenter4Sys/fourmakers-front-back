using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_apontamento_periodo_fechado_log
    {
        public int id { get; set; }
        public int? tb_apontamento_periodo_fechado_id { get; set; }
        public DateTime? data_fim_anterior { get; set; }
        public DateTime? data_fim_nova { get; set; }
        public int? tb_org_id { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }
        public string codigo_interno_colaborador_criacao { get; set; }
        public string codigo_interno_colaborador_alteracao { get; set; }

        public virtual tb_apontamento_periodo_fechado tb_apontamento_periodo_fechado { get; set; }
        public virtual tb_org tb_org { get; set; }
    }
}