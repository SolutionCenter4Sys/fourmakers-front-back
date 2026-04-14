using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_competencia_certificado
    {
        public long id { get; set; }
        public long? tb_colaborador_competencia_id { get; set; }
        public long tb_certificado_id { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public sbyte principal { get; set; }

        public virtual tb_certificado tb_certificado { get; set; }
        public virtual tb_colaborador_competencia tb_colaborador_competencia { get; set; }
    }
}