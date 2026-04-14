using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_experiencia
    {
        public tb_experiencia()
        {
            tb_experiencia_projeto = new HashSet<tb_experiencia_projeto>();
        }

        public long id { get; set; }
        public string titulo { get; set; }
        public string empresa { get; set; }
        public string descricao { get; set; }
        public DateTime data_inicio { get; set; }
        public DateTime? data_saida { get; set; }
        public sbyte atual { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string codigo_interno_colaborador { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual ICollection<tb_experiencia_projeto> tb_experiencia_projeto { get; set; }
    }
}