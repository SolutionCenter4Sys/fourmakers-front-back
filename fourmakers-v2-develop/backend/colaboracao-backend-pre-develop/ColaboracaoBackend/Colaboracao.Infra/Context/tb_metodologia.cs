using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_metodologia
    {
        public tb_metodologia()
        {
            tb_colaborador_metodologia = new HashSet<tb_colaborador_metodologia>();
            tb_filtro_metodologia_nivel = new HashSet<tb_filtro_metodologia_nivel>();
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
        public virtual ICollection<tb_colaborador_metodologia> tb_colaborador_metodologia { get; set; }
        public virtual ICollection<tb_filtro_metodologia_nivel> tb_filtro_metodologia_nivel { get; set; }
        public virtual ICollection<tb_vaga_competencia> tb_vaga_competencia { get; set; }
    }
}