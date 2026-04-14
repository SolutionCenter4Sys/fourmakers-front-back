using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_vaga_favorito
    {
        public long id { get; set; }
        public long tb_vagas_srs_id { get; set; }
        public long tb_usuario_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_usuario tb_usuario { get; set; }
        public virtual tb_vagas_srs tb_vagas_srs { get; set; }
    }
}