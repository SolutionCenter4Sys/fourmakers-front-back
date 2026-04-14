using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_noticia
    {
        public tb_noticia()
        {
            tb_like_noticia = new HashSet<tb_like_noticia>();
        }

        public long id { get; set; }
        public string titulo { get; set; }
        public string descricao { get; set; }
        public long? imagem_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_imagem imagem { get; set; }
        public virtual ICollection<tb_like_noticia> tb_like_noticia { get; set; }
    }
}