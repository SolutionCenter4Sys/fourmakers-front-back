using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_status_colaborador
    {
        public tb_status_colaborador()
        {
            tb_colaborador_status = new HashSet<tb_colaborador_status>();
        }

        public int id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual ICollection<tb_colaborador_status> tb_colaborador_status { get; set; }
    }
}