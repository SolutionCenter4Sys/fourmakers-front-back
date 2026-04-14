using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_contato_colaborador
    {
        public long id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string seguidor_codigo_interno_colaborador { get; set; }
        public string seguindo_codigo_interno_colaborador { get; set; }

        public virtual tb_colaborador seguidor_codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_colaborador seguindo_codigo_interno_colaboradorNavigation { get; set; }
    }
}