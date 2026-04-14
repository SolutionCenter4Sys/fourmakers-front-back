using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_filtro_hobbies
    {
        public long id { get; set; }
        public long filtro_id { get; set; }
        public long hobbies_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_filtro filtro { get; set; }
        public virtual tb_hobbies hobbies { get; set; }
    }
}