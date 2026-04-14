using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_filtro
    {
        public tb_filtro()
        {
            tb_filtro_competencia_nivel = new HashSet<tb_filtro_competencia_nivel>();
            tb_filtro_dominionegocio_nivel = new HashSet<tb_filtro_dominionegocio_nivel>();
            tb_filtro_formacao_nivel = new HashSet<tb_filtro_formacao_nivel>();
            tb_filtro_hobbies = new HashSet<tb_filtro_hobbies>();
            tb_filtro_interesse = new HashSet<tb_filtro_interesse>();
            tb_filtro_metodologia_nivel = new HashSet<tb_filtro_metodologia_nivel>();
            tb_filtro_modeloreferencia_nivel = new HashSet<tb_filtro_modeloreferencia_nivel>();
            tb_filtro_softskills = new HashSet<tb_filtro_softskills>();
        }

        public long id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual ICollection<tb_filtro_competencia_nivel> tb_filtro_competencia_nivel { get; set; }
        public virtual ICollection<tb_filtro_dominionegocio_nivel> tb_filtro_dominionegocio_nivel { get; set; }
        public virtual ICollection<tb_filtro_formacao_nivel> tb_filtro_formacao_nivel { get; set; }
        public virtual ICollection<tb_filtro_hobbies> tb_filtro_hobbies { get; set; }
        public virtual ICollection<tb_filtro_interesse> tb_filtro_interesse { get; set; }
        public virtual ICollection<tb_filtro_metodologia_nivel> tb_filtro_metodologia_nivel { get; set; }
        public virtual ICollection<tb_filtro_modeloreferencia_nivel> tb_filtro_modeloreferencia_nivel { get; set; }
        public virtual ICollection<tb_filtro_softskills> tb_filtro_softskills { get; set; }
    }
}