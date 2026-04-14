using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_parametro_configuracao
    {
        public Guid id { get; set; }
        public int? tb_org_id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public int? tb_grupo_acesso_id { get; set; }
        public string codigo_parametro { get; set; }
        public string valor_parametro { get; set; }
        public int? tb_parametro_nivel_id { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }

        public virtual tb_colaborador_org tb_colaborador_org { get; set; }
        public virtual tb_grupo_acesso tb_grupo_acesso { get; set; }
        public virtual tb_org tb_org { get; set; }
        public virtual tb_parametro_nivel tb_parametro_nivel { get; set; }
    }
}