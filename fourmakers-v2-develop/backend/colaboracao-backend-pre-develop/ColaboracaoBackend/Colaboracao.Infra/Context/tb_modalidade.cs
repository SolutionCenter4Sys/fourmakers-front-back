using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_modalidade
    {
        public tb_modalidade()
        {
            tb_colaborador_projeto = new HashSet<tb_colaborador_projeto>();
        }

        public long id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual ICollection<tb_colaborador_projeto> tb_colaborador_projeto { get; set; }
    }
}