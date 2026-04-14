using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_nacionalidade
    {
        public tb_nacionalidade()
        {
            tb_colaborador_passaporte = new HashSet<tb_colaborador_passaporte>();
        }

        public int Id { get; set; }
        public string Descricao { get; set; }

        public virtual ICollection<tb_colaborador_passaporte> tb_colaborador_passaporte { get; set; }
    }
}