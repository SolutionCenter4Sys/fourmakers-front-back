using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_tipo_diploma
    {
        public tb_tipo_diploma()
        {
            tb_escolaridade = new HashSet<tb_escolaridade>();
        }

        public int id { get; set; }
        public string descricao { get; set; }

        public virtual ICollection<tb_escolaridade> tb_escolaridade { get; set; }
    }
}