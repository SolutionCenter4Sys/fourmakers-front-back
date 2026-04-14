using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_filtro_metodologia_nivel
    {
        public long id { get; set; }
        public long filtro_id { get; set; }
        public long metodologia_id { get; set; }
        public long? tb_nivel_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_filtro filtro { get; set; }
        public virtual tb_metodologia metodologia { get; set; }
        public virtual tb_nivel tb_nivel { get; set; }
    }
}