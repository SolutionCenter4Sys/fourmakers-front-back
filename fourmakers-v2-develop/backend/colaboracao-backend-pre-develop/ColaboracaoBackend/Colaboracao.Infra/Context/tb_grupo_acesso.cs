using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_grupo_acesso
    {
        public tb_grupo_acesso()
        {
            tb_grupo_acesso_funcionalidade_sistema = new HashSet<tb_grupo_acesso_funcionalidade_sistema>();
            tb_parametro_configuracao = new HashSet<tb_parametro_configuracao>();
            tb_usuario_grupo_acesso = new HashSet<tb_usuario_grupo_acesso>();
        }

        public int id { get; set; }
        public string descricao { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public sbyte ativo { get; set; }
        public int nivel { get; set; }
        public int tb_org_id { get; set; }

        public virtual tb_org tb_org { get; set; }
        public virtual ICollection<tb_grupo_acesso_funcionalidade_sistema> tb_grupo_acesso_funcionalidade_sistema { get; set; }
        public virtual ICollection<tb_parametro_configuracao> tb_parametro_configuracao { get; set; }
        public virtual ICollection<tb_usuario_grupo_acesso> tb_usuario_grupo_acesso { get; set; }
    }
}