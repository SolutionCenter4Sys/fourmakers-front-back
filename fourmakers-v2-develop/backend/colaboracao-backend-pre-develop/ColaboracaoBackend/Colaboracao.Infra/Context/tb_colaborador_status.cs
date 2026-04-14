using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_status
    {
        public long id { get; set; }
        public int status_colaborador_id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_status_colaborador status_colaborador { get; set; }
    }
}