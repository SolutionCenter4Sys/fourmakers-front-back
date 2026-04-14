using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_sobre
    {
        public long id { get; set; }
        public string descricao { get; set; }
        public DateTime data_criacao { get; set; }
        public string codigo_interno_colaborador { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
    }
}