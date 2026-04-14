using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_modeloreferencia
    {
        public tb_modeloreferencia()
        {
            tb_colaborador_modeloreferencia = new HashSet<tb_colaborador_modeloreferencia>();
            tb_filtro_modeloreferencia_nivel = new HashSet<tb_filtro_modeloreferencia_nivel>();
        }

        public long id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long usuario_criacao_id { get; set; }
        public sbyte confirmada { get; set; }

        public virtual tb_usuario usuario_criacao { get; set; }
        public virtual ICollection<tb_colaborador_modeloreferencia> tb_colaborador_modeloreferencia { get; set; }
        public virtual ICollection<tb_filtro_modeloreferencia_nivel> tb_filtro_modeloreferencia_nivel { get; set; }
    }
}