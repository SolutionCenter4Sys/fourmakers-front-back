using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_imagem
    {
        public tb_imagem()
        {
            tb_colaborador = new HashSet<tb_colaborador>();
            tb_noticia = new HashSet<tb_noticia>();
        }

        public long id { get; set; }
        public string path { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual ICollection<tb_colaborador> tb_colaborador { get; set; }
        public virtual ICollection<tb_noticia> tb_noticia { get; set; }
    }
}