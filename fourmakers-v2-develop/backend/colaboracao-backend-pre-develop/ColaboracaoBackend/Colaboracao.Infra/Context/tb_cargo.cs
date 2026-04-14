using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_cargo
    {
        public tb_cargo()
        {
            tb_colaborador_cargo = new HashSet<tb_colaborador_cargo>();
        }

        public int id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual ICollection<tb_colaborador_cargo> tb_colaborador_cargo { get; set; }
    }
}