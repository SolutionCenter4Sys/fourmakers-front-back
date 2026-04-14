using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_bancos
    {
        public tb_bancos()
        {
            tb_pessoa_juridica = new HashSet<tb_pessoa_juridica>();
        }

        public string codigo { get; set; }
        public string nome { get; set; }

        public virtual ICollection<tb_pessoa_juridica> tb_pessoa_juridica { get; set; }
    }
}