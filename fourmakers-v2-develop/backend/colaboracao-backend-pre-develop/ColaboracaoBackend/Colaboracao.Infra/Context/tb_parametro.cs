using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_parametro
    {
        public Guid id { get; set; }
        public string nome_parametro { get; set; }
        public string descricao_parametro { get; set; }
        public string codigo_parametro { get; set; }
        public string codigo_modulo_sistema { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }
        public bool? ativo { get; set; }
    }
}