using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_formacao
    {
        public tb_colaborador_formacao()
        {
            tb_endosso_formacao = new HashSet<tb_endosso_formacao>();
            tb_like_formacao = new HashSet<tb_like_formacao>();
        }

        public long id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long formacao_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public long? tb_certificado_id { get; set; }
        public long? tb_nivel_id { get; set; }
        public DateTime? data_conclusao { get; set; }
        public string emissor { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_formacao formacao { get; set; }
        public virtual tb_certificado tb_certificado { get; set; }
        public virtual tb_nivel tb_nivel { get; set; }
        public virtual ICollection<tb_endosso_formacao> tb_endosso_formacao { get; set; }
        public virtual ICollection<tb_like_formacao> tb_like_formacao { get; set; }
    }
}