using System.Collections.Generic;

namespace Colaboracao.Infra.Context
{
    public partial class tb_origem_historico_cv
    {
        public tb_origem_historico_cv()
        {
            tb_historico_cv = new HashSet<tb_historico_cv>();
        }

        public string id { get; set; }
        public string descricao { get; set; }

        public virtual ICollection<tb_historico_cv> tb_historico_cv { get; set; }
    }
}