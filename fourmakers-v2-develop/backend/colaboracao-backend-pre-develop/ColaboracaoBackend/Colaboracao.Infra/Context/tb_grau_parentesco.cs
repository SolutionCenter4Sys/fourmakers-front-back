using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_grau_parentesco
    {
        public int id { get; set; }
        public string descricao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
    }
}