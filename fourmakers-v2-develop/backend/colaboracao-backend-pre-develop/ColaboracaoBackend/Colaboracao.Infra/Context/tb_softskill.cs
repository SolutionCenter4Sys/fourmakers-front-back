using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_softskill
    {
        public tb_softskill()
        {
            tb_colaborador_softskill = new HashSet<tb_colaborador_softskill>();
            tb_filtro_softskills = new HashSet<tb_filtro_softskills>();
            tb_vaga_competencia = new HashSet<tb_vaga_competencia>();
        }

        public long id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }
        public long? usuario_criacao_id { get; set; }
        public sbyte confirmada { get; set; }

        public virtual ICollection<tb_colaborador_softskill> tb_colaborador_softskill { get; set; }
        public virtual ICollection<tb_filtro_softskills> tb_filtro_softskills { get; set; }
        public virtual ICollection<tb_vaga_competencia> tb_vaga_competencia { get; set; }
    }
}