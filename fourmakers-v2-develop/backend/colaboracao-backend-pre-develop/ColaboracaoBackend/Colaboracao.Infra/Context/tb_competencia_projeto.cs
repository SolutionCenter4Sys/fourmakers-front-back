using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_competencia_projeto
    {
        public long projeto_id { get; set; }
        public long competencia_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_competencia competencia { get; set; }
        public virtual tb_projeto projeto { get; set; }
    }
}