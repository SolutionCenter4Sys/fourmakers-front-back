using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_video
    {
        public long id { get; set; }
        public string titulo { get; set; }
        public string descricao { get; set; }
        public string path { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long tb_acesso_id { get; set; }

        public virtual tb_acesso tb_acesso { get; set; }
    }
}