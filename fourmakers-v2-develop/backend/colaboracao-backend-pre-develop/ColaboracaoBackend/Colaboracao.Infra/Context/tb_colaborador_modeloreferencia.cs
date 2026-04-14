using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_modeloreferencia
    {
        public tb_colaborador_modeloreferencia()
        {
            tb_endosso_modeloreferencia = new HashSet<tb_endosso_modeloreferencia>();
            tb_like_modeloreferencia = new HashSet<tb_like_modeloreferencia>();
        }

        public long id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long modeloreferencia_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long? tb_nivel_id { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_modeloreferencia modeloreferencia { get; set; }
        public virtual tb_nivel tb_nivel { get; set; }
        public virtual ICollection<tb_endosso_modeloreferencia> tb_endosso_modeloreferencia { get; set; }
        public virtual ICollection<tb_like_modeloreferencia> tb_like_modeloreferencia { get; set; }
    }
}