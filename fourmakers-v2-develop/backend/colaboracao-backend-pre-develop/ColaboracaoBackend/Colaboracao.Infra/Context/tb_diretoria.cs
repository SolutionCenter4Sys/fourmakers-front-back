using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_diretoria
    {
        public tb_diretoria()
        {
            tb_colaborador = new HashSet<tb_colaborador>();
        }

        public int id { get; set; }
        public string descricao { get; set; }
        public long org_id { get; set; }
        public string id_externo { get; set; }

        public virtual tb_org org { get; set; }
        public virtual ICollection<tb_colaborador> tb_colaborador { get; set; }
    }
}