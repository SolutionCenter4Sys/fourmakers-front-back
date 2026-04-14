using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_vigencia
    {
        public tb_vigencia()
        {
            tb_colaborador_apontamento = new HashSet<tb_colaborador_apontamento>();
            tb_colaborador_apontamento_log = new HashSet<tb_colaborador_apontamento_log>();
        }

        public Guid id { get; set; }
        public int mes { get; set; }
        public int ano { get; set; }

        public virtual ICollection<tb_colaborador_apontamento> tb_colaborador_apontamento { get; set; }
        public virtual ICollection<tb_colaborador_apontamento_log> tb_colaborador_apontamento_log { get; set; }
    }
}