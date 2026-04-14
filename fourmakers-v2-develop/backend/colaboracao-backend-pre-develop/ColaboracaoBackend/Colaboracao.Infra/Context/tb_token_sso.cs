using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_token_sso
    {
        public long id { get; set; }
        public string token { get; set; }
        public string refresh_token { get; set; }
        public long tb_usuario_id { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_usuario tb_usuario { get; set; }
    }
}