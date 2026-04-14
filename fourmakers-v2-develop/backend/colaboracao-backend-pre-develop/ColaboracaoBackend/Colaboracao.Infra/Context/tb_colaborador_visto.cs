using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_visto
    {
        public int id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public int tb_pais_id { get; set; }
        public DateTime validade { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_pais tb_pais { get; set; }
    }
}