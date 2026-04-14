using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_endosso_metodologia
    {
        public long id { get; set; }
        public int? tb_tipo_endosso_id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long colaborador_metodologia_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public int? tb_status_endosso_id { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_colaborador_metodologia colaborador_metodologia { get; set; }
        public virtual tb_status_endosso tb_status_endosso { get; set; }
        public virtual tb_tipo_endosso tb_tipo_endosso { get; set; }
    }
}