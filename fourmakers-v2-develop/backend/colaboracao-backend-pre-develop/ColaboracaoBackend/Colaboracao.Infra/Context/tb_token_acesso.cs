using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_token_acesso
    {
        public tb_token_acesso()
        {
            tb_usuario_tokenacesso = new HashSet<tb_usuario_tokenacesso>();
        }

        public long id { get; set; }
        public string token { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime validade { get; set; }
        public sbyte ativo { get; set; }

        public virtual ICollection<tb_usuario_tokenacesso> tb_usuario_tokenacesso { get; set; }
    }
}