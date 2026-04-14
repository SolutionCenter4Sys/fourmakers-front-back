using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_funcionalidade_rota
    {
        public Guid id { get; set; }
        public string rota { get; set; }
        public int tb_funcionalidade_sistema_id { get; set; }
        public int tb_org_id { get; set; }

        public virtual tb_funcionalidade_sistema tb_funcionalidade_sistema { get; set; }
        public virtual tb_org tb_org { get; set; }
    }
}