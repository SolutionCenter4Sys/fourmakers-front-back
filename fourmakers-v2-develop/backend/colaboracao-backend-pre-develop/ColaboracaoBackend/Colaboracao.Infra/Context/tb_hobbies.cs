using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_hobbies
    {
        public tb_hobbies()
        {
            tb_colaborador_hobbies = new HashSet<tb_colaborador_hobbies>();
            tb_filtro_hobbies = new HashSet<tb_filtro_hobbies>();
        }

        public long id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long usuario_criacao_id { get; set; }

        public virtual tb_usuario usuario_criacao { get; set; }
        public virtual ICollection<tb_colaborador_hobbies> tb_colaborador_hobbies { get; set; }
        public virtual ICollection<tb_filtro_hobbies> tb_filtro_hobbies { get; set; }
    }
}