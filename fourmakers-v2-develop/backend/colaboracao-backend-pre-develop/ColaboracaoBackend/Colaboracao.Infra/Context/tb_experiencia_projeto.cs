using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_experiencia_projeto
    {
        public long id { get; set; }
        public string nome_projeto { get; set; }
        public DateTime? data_inicio { get; set; }
        public DateTime? data_fim { get; set; }
        public long experiencia_id { get; set; }

        public virtual tb_experiencia experiencia { get; set; }
    }
}