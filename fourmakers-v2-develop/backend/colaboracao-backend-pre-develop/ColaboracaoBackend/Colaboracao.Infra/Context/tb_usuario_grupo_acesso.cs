using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_usuario_grupo_acesso
    {
        public int id { get; set; }
        public long tb_usuario_id { get; set; }
        public int tb_grupo_acesso_id { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public sbyte ativo { get; set; }

        public virtual tb_grupo_acesso tb_grupo_acesso { get; set; }
        public virtual tb_usuario tb_usuario { get; set; }
    }
}