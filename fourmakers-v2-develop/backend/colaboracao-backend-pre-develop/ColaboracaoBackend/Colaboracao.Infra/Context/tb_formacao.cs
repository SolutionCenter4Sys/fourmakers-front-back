using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_formacao
    {
        public tb_formacao()
        {
            tb_colaborador_formacao = new HashSet<tb_colaborador_formacao>();
            tb_escolaridade = new HashSet<tb_escolaridade>();
            tb_filtro_formacao_nivel = new HashSet<tb_filtro_formacao_nivel>();
        }

        public long id { get; set; }
        public string descricao { get; set; }
        public string emissor { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long usuario_criacao_id { get; set; }

        public virtual tb_usuario usuario_criacao { get; set; }
        public virtual ICollection<tb_colaborador_formacao> tb_colaborador_formacao { get; set; }
        public virtual ICollection<tb_escolaridade> tb_escolaridade { get; set; }
        public virtual ICollection<tb_filtro_formacao_nivel> tb_filtro_formacao_nivel { get; set; }
    }
}