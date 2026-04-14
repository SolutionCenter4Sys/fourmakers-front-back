using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_item_perfil
    {
        public tb_item_perfil()
        {
            tb_nivel = new HashSet<tb_nivel>();
            tb_historico_cv = new HashSet<tb_historico_cv>();
        }

        public long id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual ICollection<tb_nivel> tb_nivel { get; set; }
        public virtual ICollection<tb_historico_cv> tb_historico_cv { get; set; }
    }
}