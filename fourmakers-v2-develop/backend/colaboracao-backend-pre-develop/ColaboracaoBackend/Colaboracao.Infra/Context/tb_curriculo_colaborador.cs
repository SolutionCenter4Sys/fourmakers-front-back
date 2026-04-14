using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_curriculo_colaborador
    {
        public long id { get; set; }
        public string path_curriculo { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string codigo_interno_colaborador { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
    }
}