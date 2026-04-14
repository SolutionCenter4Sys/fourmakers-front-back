using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_acesso
    {
        public tb_acesso()
        {
            tb_video = new HashSet<tb_video>();
        }

        public long id { get; set; }
        public string status { get; set; }

        public virtual ICollection<tb_video> tb_video { get; set; }
    }
}