using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_comentario
    {
        public long id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public int comentario_tipo_id { get; set; }
        public string texto { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_comentario_tipo comentario_tipo { get; set; }
    }
}