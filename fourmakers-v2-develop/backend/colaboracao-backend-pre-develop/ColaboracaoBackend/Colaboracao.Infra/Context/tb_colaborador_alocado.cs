using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_alocado
    {
        public tb_colaborador_alocado()
        {
            tb_periodo_alocacao = new HashSet<tb_periodo_alocacao>();
        }

        public long id { get; set; }
        public string codigo_colaborador { get; set; }
        public string email_gestor { get; set; }
        public string codigo_projeto { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public int tb_org_id { get; set; }
        public int? cod_tbd_alocado { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_org tb_org { get; set; }
        public virtual ICollection<tb_periodo_alocacao> tb_periodo_alocacao { get; set; }
    }
}