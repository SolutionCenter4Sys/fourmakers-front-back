using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_competencia
    {
        public tb_competencia()
        {
            tb_colaborador_competencia = new HashSet<tb_colaborador_competencia>();
            tb_colaborador_referencia_hardskill = new HashSet<tb_colaborador_referencia_hardskill>();
            tb_filtro_competencia_nivel = new HashSet<tb_filtro_competencia_nivel>();
            tb_vaga_competencia = new HashSet<tb_vaga_competencia>();
        }

        public long id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }
        public long usuario_criacao_id { get; set; }
        public sbyte confirmada { get; set; }

        public virtual tb_usuario usuario_criacao { get; set; }
        public virtual ICollection<tb_colaborador_competencia> tb_colaborador_competencia { get; set; }
        public virtual ICollection<tb_colaborador_referencia_hardskill> tb_colaborador_referencia_hardskill { get; set; }
        public virtual ICollection<tb_filtro_competencia_nivel> tb_filtro_competencia_nivel { get; set; }
        public virtual ICollection<tb_vaga_competencia> tb_vaga_competencia { get; set; }
    }
}