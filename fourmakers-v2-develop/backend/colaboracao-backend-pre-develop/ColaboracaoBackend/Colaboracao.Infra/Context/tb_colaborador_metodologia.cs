using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_metodologia
    {
        public tb_colaborador_metodologia()
        {
            tb_endosso_metodologia = new HashSet<tb_endosso_metodologia>();
            tb_like_metodologia = new HashSet<tb_like_metodologia>();
        }

        public long id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long metodologia_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long? tb_nivel_id { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_metodologia metodologia { get; set; }
        public virtual tb_nivel tb_nivel { get; set; }
        public virtual ICollection<tb_endosso_metodologia> tb_endosso_metodologia { get; set; }
        public virtual ICollection<tb_like_metodologia> tb_like_metodologia { get; set; }
    }
}