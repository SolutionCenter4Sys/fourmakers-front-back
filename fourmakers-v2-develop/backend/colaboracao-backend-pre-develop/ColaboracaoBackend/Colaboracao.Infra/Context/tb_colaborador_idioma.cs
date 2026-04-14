using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_idioma
    {
        public int id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public int idioma_id { get; set; }
        public long? tb_nivel_id { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_idioma idioma { get; set; }
        public virtual tb_nivel tb_nivel { get; set; }
    }
}