using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_interesse
    {
        public tb_interesse()
        {
            //tb_colaborador_interesse = new HashSet<tb_colaborador_interesse>();
            tb_filtro_interesse = new HashSet<tb_filtro_interesse>();
        }

        public long id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long usuario_criacao_id { get; set; }

        public virtual tb_usuario usuario_criacao { get; set; }
        //public virtual ICollection<tb_colaborador_interesse> tb_colaborador_interesse { get; set; }
        public virtual ICollection<tb_filtro_interesse> tb_filtro_interesse { get; set; }
    }
}