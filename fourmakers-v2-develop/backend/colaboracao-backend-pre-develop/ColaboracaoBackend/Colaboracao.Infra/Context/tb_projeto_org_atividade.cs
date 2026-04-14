using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_projeto_org_atividade
    {
        public Guid id { get; set; }
        public string tb_projeto_org_cod_projeto { get; set; }
        public int? tb_projeto_tb_org_id { get; set; }
        public Guid? tb_atividade_id { get; set; }

        public virtual tb_atividade tb_atividade { get; set; }
        public virtual tb_projeto_org tb_projeto_ { get; set; }
    }
}