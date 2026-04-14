using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_parametro_nivel
    {
        public tb_parametro_nivel()
        {
            tb_parametro_configuracao = new HashSet<tb_parametro_configuracao>();
        }

        public int id { get; set; }
        public string descricao_nivel { get; set; }
        public int? prioridade { get; set; }

        public virtual ICollection<tb_parametro_configuracao> tb_parametro_configuracao { get; set; }
    }
}