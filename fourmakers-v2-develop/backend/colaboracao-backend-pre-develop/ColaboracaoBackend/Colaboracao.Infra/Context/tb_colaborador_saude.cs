using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_saude
    {
        public tb_colaborador_saude()
        {
            tb_colaborador = new HashSet<tb_colaborador>();
        }

        public int id { get; set; }
        public string pcd { get; set; }
        public sbyte? grupo_risco_covid { get; set; }
        public string condicao_saude_relevante { get; set; }

        public virtual ICollection<tb_colaborador> tb_colaborador { get; set; }
    }
}