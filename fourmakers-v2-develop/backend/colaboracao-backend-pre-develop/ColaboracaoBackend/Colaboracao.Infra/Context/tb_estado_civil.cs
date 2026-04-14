using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_estado_civil
    {
        public int id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
    }
}