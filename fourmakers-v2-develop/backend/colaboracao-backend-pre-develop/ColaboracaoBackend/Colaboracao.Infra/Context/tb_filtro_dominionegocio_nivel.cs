using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_filtro_dominionegocio_nivel
    {
        public long id { get; set; }
        public long filtro_id { get; set; }
        public long dominionegocio_id { get; set; }
        public long? nivel_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_dominionegocio dominionegocio { get; set; }
        public virtual tb_filtro filtro { get; set; }
        public virtual tb_nivel nivel { get; set; }
    }
}