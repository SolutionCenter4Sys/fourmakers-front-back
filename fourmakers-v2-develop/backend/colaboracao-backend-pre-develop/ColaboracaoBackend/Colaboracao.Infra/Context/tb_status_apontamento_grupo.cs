using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_status_apontamento_grupo
    {
        public tb_status_apontamento_grupo()
        {
            tb_status_apontamento = new HashSet<tb_status_apontamento>();
        }

        public Guid id { get; set; }
        public string descricao { get; set; }
        public int? cod_status_grupo { get; set; }
        public int? prioridade { get; set; }

        public virtual ICollection<tb_status_apontamento> tb_status_apontamento { get; set; }
    }
}