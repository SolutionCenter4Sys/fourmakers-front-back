using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_certificado
    {
        public tb_certificado()
        {
            tb_colaborador_competencia_certificado = new HashSet<tb_colaborador_competencia_certificado>();
            tb_colaborador_formacao = new HashSet<tb_colaborador_formacao>();
        }

        public long id { get; set; }
        public string path { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public DateTime? data_conclusao { get; set; }
        public string descricao { get; set; }
        public string instituicao { get; set; }
        public int carga_horaria { get; set; }
        public string codigo_interno_colaborador { get; set; }

        public virtual ICollection<tb_colaborador_competencia_certificado> tb_colaborador_competencia_certificado { get; set; }
        public virtual ICollection<tb_colaborador_formacao> tb_colaborador_formacao { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
    }
}