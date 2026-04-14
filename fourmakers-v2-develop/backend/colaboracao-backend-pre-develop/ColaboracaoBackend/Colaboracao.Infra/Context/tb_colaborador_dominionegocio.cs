using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_dominionegocio
    {
        public tb_colaborador_dominionegocio()
        {
            tb_endosso_dominionegocio = new HashSet<tb_endosso_dominionegocio>();
            tb_like_dominionegocio = new HashSet<tb_like_dominionegocio>();
        }

        public long id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long dominionegocio_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long? tb_nivel_id { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_dominionegocio dominionegocio { get; set; }
        public virtual tb_nivel tb_nivel { get; set; }
        public virtual ICollection<tb_endosso_dominionegocio> tb_endosso_dominionegocio { get; set; }
        public virtual ICollection<tb_like_dominionegocio> tb_like_dominionegocio { get; set; }
    }
}