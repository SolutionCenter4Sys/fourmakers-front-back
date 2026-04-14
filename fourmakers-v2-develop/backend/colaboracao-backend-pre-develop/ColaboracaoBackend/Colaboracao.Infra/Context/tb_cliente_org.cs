using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_cliente_org
    {
        public Guid id { get; set; }
        public string codigo_cliente { get; set; }
        public string nome_cliente { get; set; }
        public int? tb_org_id { get; set; }
        public bool? ativo { get; set; }
        public string tipo_cadastro { get; set; }
        public DateTime? data_criacao { get; set; }
        public DateTime? data_alteracao { get; set; }

        public virtual tb_org tb_org { get; set; }
        public bool deve_ocultar_na_gestao_de_alocados { get; set; }
    }
}