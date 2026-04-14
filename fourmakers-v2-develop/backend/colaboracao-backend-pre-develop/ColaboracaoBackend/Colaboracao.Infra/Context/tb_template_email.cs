using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_template_email
    {
        public Guid id { get; set; }
        public string codigo { get; set; }
        public string descricao { get; set; }
        public string template { get; set; }
        public sbyte? ativo { get; set; }
        public DateTime? data_alteracao { get; set; }
        public DateTime? data_criacao { get; set; }
    }
}