using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_atividade
    {
        public tb_atividade()
        {
            tb_colaborador_apontamento = new HashSet<tb_colaborador_apontamento>();
            tb_colaborador_periodo_alocacao = new HashSet<tb_colaborador_periodo_alocacao>();
            tb_projeto_org_atividade = new HashSet<tb_projeto_org_atividade>();
        }

        public Guid id { get; set; }
        public string descricao { get; set; }
        public int tb_org_id { get; set; }

        public virtual tb_org tb_org { get; set; }
        public virtual ICollection<tb_colaborador_apontamento> tb_colaborador_apontamento { get; set; }
        public virtual ICollection<tb_colaborador_periodo_alocacao> tb_colaborador_periodo_alocacao { get; set; }
        public virtual ICollection<tb_projeto_org_atividade> tb_projeto_org_atividade { get; set; }
    }
}