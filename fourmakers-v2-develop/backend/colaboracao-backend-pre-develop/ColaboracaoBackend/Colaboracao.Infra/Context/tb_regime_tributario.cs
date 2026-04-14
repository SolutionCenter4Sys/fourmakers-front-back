using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_regime_tributario
    {
        public tb_regime_tributario()
        {
            tb_pessoa_juridica = new HashSet<tb_pessoa_juridica>();
        }

        public int id { get; set; }
        public string descricao { get; set; }

        public virtual ICollection<tb_pessoa_juridica> tb_pessoa_juridica { get; set; }
    }
}