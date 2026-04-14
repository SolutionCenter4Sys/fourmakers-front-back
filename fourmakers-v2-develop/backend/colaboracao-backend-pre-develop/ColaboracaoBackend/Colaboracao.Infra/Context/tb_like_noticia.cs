using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_like_noticia
    {
        public string codigo_interno_colaborador { get; set; }
        public long tb_noticia_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_noticia tb_noticia { get; set; }
    }
}