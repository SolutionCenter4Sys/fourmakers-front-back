using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_comentario_tipo
    {
        public tb_comentario_tipo()
        {
            tb_colaborador_comentario = new HashSet<tb_colaborador_comentario>();
        }

        public int id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual ICollection<tb_colaborador_comentario> tb_colaborador_comentario { get; set; }
    }
}