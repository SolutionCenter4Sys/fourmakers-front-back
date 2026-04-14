using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_cargo
    {
        public tb_colaborador_cargo()
        {
            tb_like_cargo = new HashSet<tb_like_cargo>();
        }

        public long id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public int cargo_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_cargo cargo { get; set; }
        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual ICollection<tb_like_cargo> tb_like_cargo { get; set; }
    }
}