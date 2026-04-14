using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_referencia_hardskill
    {
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long tb_competencia_id { get; set; }
        public int tb_nivel_referencia_hardskill_id { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_competencia tb_competencia { get; set; }
        public virtual tb_nivel_referencia_hardskill tb_nivel_referencia_hardskill { get; set; }
    }
}