using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_funcionalidade_sistema
    {
        public tb_funcionalidade_sistema()
        {
            tb_funcionalidade_rota = new HashSet<tb_funcionalidade_rota>();
            tb_grupo_acesso_funcionalidade_sistema = new HashSet<tb_grupo_acesso_funcionalidade_sistema>();
            tb_notificacao = new HashSet<tb_notificacao>();
        }

        public int id { get; set; }
        public string descricao { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public sbyte ativo { get; set; }

        public virtual ICollection<tb_funcionalidade_rota> tb_funcionalidade_rota { get; set; }
        public virtual ICollection<tb_grupo_acesso_funcionalidade_sistema> tb_grupo_acesso_funcionalidade_sistema { get; set; }
        public virtual ICollection<tb_notificacao> tb_notificacao { get; set; }
    }
}