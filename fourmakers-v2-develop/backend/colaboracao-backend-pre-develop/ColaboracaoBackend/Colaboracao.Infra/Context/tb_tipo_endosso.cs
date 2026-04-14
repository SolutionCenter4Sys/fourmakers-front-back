using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_tipo_endosso
    {
        public tb_tipo_endosso()
        {
            tb_endosso_competencia = new HashSet<tb_endosso_competencia>();
            tb_endosso_dominionegocio = new HashSet<tb_endosso_dominionegocio>();
            tb_endosso_formacao = new HashSet<tb_endosso_formacao>();
            tb_endosso_metodologia = new HashSet<tb_endosso_metodologia>();
            tb_endosso_modeloreferencia = new HashSet<tb_endosso_modeloreferencia>();
        }

        public int id { get; set; }
        public string descricao { get; set; }
        public int nivel { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual ICollection<tb_endosso_competencia> tb_endosso_competencia { get; set; }
        public virtual ICollection<tb_endosso_dominionegocio> tb_endosso_dominionegocio { get; set; }
        public virtual ICollection<tb_endosso_formacao> tb_endosso_formacao { get; set; }
        public virtual ICollection<tb_endosso_metodologia> tb_endosso_metodologia { get; set; }
        public virtual ICollection<tb_endosso_modeloreferencia> tb_endosso_modeloreferencia { get; set; }
    }
}