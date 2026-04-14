using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_like_modeloreferencia
    {
        public string codigo_interno_colaborador { get; set; }
        public long colaborador_modeloreferencia_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_colaborador_modeloreferencia colaborador_modeloreferencia { get; set; }
    }
}