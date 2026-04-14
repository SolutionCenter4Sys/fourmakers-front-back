using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_skill_vaga
    {
        public long id { get; set; }
        public int tipo_skill_id { get; set; }
        public int skill_id { get; set; }
        public int skill_nivel_id { get; set; }
        public long tb_vagas_srs_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_vagas_srs tb_vagas_srs { get; set; }
    }
}