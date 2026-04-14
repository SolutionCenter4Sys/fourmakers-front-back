using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_graugraduacao
    {
        public tb_graugraduacao()
        {
            tb_colaborador_graugraduacao = new HashSet<tb_colaborador_graugraduacao>();
        }

        public long id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long usuario_criacao_id { get; set; }

        public virtual tb_usuario usuario_criacao { get; set; }
        public virtual ICollection<tb_colaborador_graugraduacao> tb_colaborador_graugraduacao { get; set; }
    }
}