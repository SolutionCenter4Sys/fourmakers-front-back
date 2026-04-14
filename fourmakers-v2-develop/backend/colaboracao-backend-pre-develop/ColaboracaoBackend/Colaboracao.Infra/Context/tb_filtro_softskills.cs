using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_filtro_softskills
    {
        public long id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long tb_filtro_id { get; set; }
        public long tb_softskill_id { get; set; }
        public long? tb_nivel_id { get; set; }

        public virtual tb_filtro tb_filtro { get; set; }
        public virtual tb_nivel tb_nivel { get; set; }
        public virtual tb_softskill tb_softskill { get; set; }
    }
}