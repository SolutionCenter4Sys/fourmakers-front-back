using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_tbd_alocado
    {
        public tb_tbd_alocado()
        {
            tb_colaborador_periodo_alocacao = new HashSet<tb_colaborador_periodo_alocacao>();
        }
        public int cod_tbd_alocado { get; set; }
        public string descricao { get; set; }
        public string codigo_interno_colaborador_gestor { get; set; }
        public string cod_diretoria { get; set; }
        public string diretoria { get; set; }
        public int? tb_org_id { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }

        public virtual tb_colaborador codigo_interno_colaborador_gestorNavigation { get; set; }
        public virtual tb_org tb_org { get; set; }

        public virtual ICollection<tb_colaborador_periodo_alocacao> tb_colaborador_periodo_alocacao { get; set; }
    }
}