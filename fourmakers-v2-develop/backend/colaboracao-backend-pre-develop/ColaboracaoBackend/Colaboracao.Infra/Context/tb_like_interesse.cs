using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_like_interesse
    {
        public string codigo_interno_colaborador { get; set; }
        public long colaborador_interesse_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        //public virtual tb_colaborador_interesse colaborador_interesse { get; set; }
    }
}