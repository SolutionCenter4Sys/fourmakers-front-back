using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_idioma
    {
        public tb_idioma()
        {
            tb_colaborador_idioma = new HashSet<tb_colaborador_idioma>();
            tb_vaga_competencia = new HashSet<tb_vaga_competencia>();
        }

        public int id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }
        public long? usuario_criacao_id { get; set; }
        public sbyte confirmada { get; set; }

        public virtual ICollection<tb_colaborador_idioma> tb_colaborador_idioma { get; set; }
        public virtual ICollection<tb_vaga_competencia> tb_vaga_competencia { get; set; }
    }
}