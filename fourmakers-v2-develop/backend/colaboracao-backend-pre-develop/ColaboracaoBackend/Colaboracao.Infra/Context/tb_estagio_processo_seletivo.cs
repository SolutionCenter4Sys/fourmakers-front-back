using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_estagio_processo_seletivo
    {
        public tb_estagio_processo_seletivo()
        {
            tb_candidato = new HashSet<tb_candidato>();
        }

        public int id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual ICollection<tb_candidato> tb_candidato { get; set; }
    }
}