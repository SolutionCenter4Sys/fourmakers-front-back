using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_competencia
    {
        public tb_colaborador_competencia()
        {
            tb_colaborador_competencia_certificado = new HashSet<tb_colaborador_competencia_certificado>();
            tb_endosso_competencia = new HashSet<tb_endosso_competencia>();
            tb_like_competencia = new HashSet<tb_like_competencia>();
        }

        public long id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long competencia_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long? tb_nivel_id { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_competencia competencia { get; set; }
        public virtual tb_nivel tb_nivel { get; set; }
        public virtual ICollection<tb_colaborador_competencia_certificado> tb_colaborador_competencia_certificado { get; set; }
        public virtual ICollection<tb_endosso_competencia> tb_endosso_competencia { get; set; }
        public virtual ICollection<tb_like_competencia> tb_like_competencia { get; set; }
    }
}