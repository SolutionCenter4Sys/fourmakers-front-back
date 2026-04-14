using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_usuario_tokenacesso
    {
        public long id { get; set; }
        public long token_acesso_id { get; set; }
        public long usuario_id { get; set; }
        public DateTime data_criacao { get; set; }

        public virtual tb_token_acesso token_acesso { get; set; }
        public virtual tb_usuario usuario { get; set; }
    }
}