using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_apontamento
    {
        public Guid id { get; set; }
        public long horas { get; set; }
        public string justificativa { get; set; }
        public DateTime data { get; set; }
        public int? numero_semana { get; set; }
        public int? numero_semana_dia { get; set; }
        public Guid? tb_atividade_id { get; set; }
        public Guid? tb_status_apontamento_id { get; set; }
        public string tipo_apontamento_id { get; set; }
        public Guid? tb_vigencia_id { get; set; }
        public string tb_projeto_org_cod_projeto { get; set; }
        public int tb_org_id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public string observacao { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }
        public string codigo_interno_colaborador_criacao { get; set; }
        public string codigo_interno_colaborador_alteracao { get; set; }
        public string codigo_interno_colaborador_justificativa { get; set; }
        public DateTime? data_justificativa { get; set; }
        public string tb_status_descricao { get; set; }
        public string tb_status_grupo_descricao { get; set; }
        public int tb_status_grupo_codigo { get; set; }

        public virtual tb_projeto_org tb_ { get; set; }
        public virtual tb_atividade tb_atividade { get; set; }
        public virtual tb_colaborador_org tb_colaborador_org { get; set; }
        public virtual tb_status_apontamento tb_status_apontamento { get; set; }
        public virtual tb_vigencia tb_vigencia { get; set; }
    }
}