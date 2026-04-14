using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_projeto_metodologia
    {
        public long projeto_id { get; set; }
        public long metodologia_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_metodologia metodologia { get; set; }
        public virtual tb_projeto projeto { get; set; }
    }
}