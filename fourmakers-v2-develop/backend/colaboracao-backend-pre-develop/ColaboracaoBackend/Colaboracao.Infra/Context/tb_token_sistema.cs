using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_token_sistema
    {
        public int id { get; set; }
        public string sistema { get; set; }
        public string token { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public sbyte ativo { get; set; }
        public int? tb_org_id { get; set; }

        public virtual tb_org tb_org { get; set; }
    }
}