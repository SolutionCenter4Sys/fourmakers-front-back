using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_status_apontamento
    {
        public tb_status_apontamento()
        {
            tb_colaborador_apontamento = new HashSet<tb_colaborador_apontamento>();
            tb_colaborador_apontamento_logtb_status_apontamento_anterior = new HashSet<tb_colaborador_apontamento_log>();
            tb_colaborador_apontamento_logtb_status_apontamento_novo = new HashSet<tb_colaborador_apontamento_log>();
        }

        public Guid id { get; set; }
        public string descricao { get; set; }
        public int? cod_status_apontamento { get; set; }
        public int? tb_cod_status_grupo { get; set; }
        public bool? exibir_status_gerente_projeto { get; set; }

        public virtual tb_status_apontamento_grupo tb_cod_status_grupoNavigation { get; set; }
        public virtual ICollection<tb_colaborador_apontamento> tb_colaborador_apontamento { get; set; }
        public virtual ICollection<tb_colaborador_apontamento_log> tb_colaborador_apontamento_logtb_status_apontamento_anterior { get; set; }
        public virtual ICollection<tb_colaborador_apontamento_log> tb_colaborador_apontamento_logtb_status_apontamento_novo { get; set; }
    }
}