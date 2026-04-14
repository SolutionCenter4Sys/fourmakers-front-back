using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_hobbies
    {
        public tb_colaborador_hobbies()
        {
            tb_like_hobbies = new HashSet<tb_like_hobbies>();
        }

        public long id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long hobbies_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_hobbies hobbies { get; set; }
        public virtual ICollection<tb_like_hobbies> tb_like_hobbies { get; set; }
    }
}