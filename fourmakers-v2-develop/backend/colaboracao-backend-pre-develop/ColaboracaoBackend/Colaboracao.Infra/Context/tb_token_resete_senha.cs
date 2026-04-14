using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_token_resete_senha
    {
        public long id { get; set; }
        public string token { get; set; }
        public DateTime validade { get; set; }
        public long tb_usuario_id { get; set; }

        public virtual tb_usuario tb_usuario { get; set; }
    }
}