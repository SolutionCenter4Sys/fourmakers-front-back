using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_grupo_acesso_funcionalidade_sistema
    {
        public int id { get; set; }
        public int tb_grupo_acesso_id { get; set; }
        public int tb_funcionalidade_sistema_id { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public sbyte ativo { get; set; }

        public virtual tb_funcionalidade_sistema tb_funcionalidade_sistema { get; set; }
        public virtual tb_grupo_acesso tb_grupo_acesso { get; set; }
    }
}