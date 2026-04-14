using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_apontamento_log
    {
        public Guid id { get; set; }
        public DateTime? data_criacao { get; set; }
        public Guid? colaborador_apontamento_id { get; set; }
        public Guid? tb_status_apontamento_anterior_id { get; set; }
        public Guid? tb_status_apontamento_novo_id { get; set; }
        public string justificativa { get; set; }
        public ulong? horas_anterior { get; set; }
        public ulong? horas_novo { get; set; }
        public ulong? horas_reprovadas { get; set; }
        public Guid? tb_vigencia_id { get; set; }
        public string tb_projeto_org_cod_projeto { get; set; }
        public int? tb_org_id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public int? numero_semana { get; set; }
        public int? numero_semana_dia { get; set; }
        public Guid? tb_atividade_id { get; set; }
        public string codigo_interno_colaborador_criacao { get; set; }

        public virtual tb_colaborador_org tb_colaborador_org { get; set; }
        public virtual tb_status_apontamento tb_status_apontamento_anterior { get; set; }
        public virtual tb_status_apontamento tb_status_apontamento_novo { get; set; }
        public virtual tb_vigencia tb_vigencia { get; set; }
    }
}