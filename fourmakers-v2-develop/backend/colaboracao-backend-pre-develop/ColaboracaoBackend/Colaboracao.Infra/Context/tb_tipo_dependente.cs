using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_tipo_dependente
    {
        public tb_tipo_dependente()
        {
            tb_colaborador_dependente = new HashSet<tb_colaborador_dependente>();
        }

        public int id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual ICollection<tb_colaborador_dependente> tb_colaborador_dependente { get; set; }
    }
}