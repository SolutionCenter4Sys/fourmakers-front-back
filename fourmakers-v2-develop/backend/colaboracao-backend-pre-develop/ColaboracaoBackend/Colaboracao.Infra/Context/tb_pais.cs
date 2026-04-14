using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_pais
    {
        public tb_pais()
        {
            tb_colaborador_visto = new HashSet<tb_colaborador_visto>();
        }

        public int Id { get; set; }
        public string Descricao { get; set; }

        public virtual ICollection<tb_colaborador_visto> tb_colaborador_visto { get; set; }
    }
}