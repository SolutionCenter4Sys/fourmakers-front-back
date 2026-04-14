using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_feriado
    {
        public int id { get; set; }
        public DateTime data { get; set; }
        public string nome { get; set; }
        public string tipo { get; set; }
        public bool ativo { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_atualizacao { get; set; }
        public int tb_org_id { get; set; }

        public virtual tb_org tb_org { get; set; }
    }
}