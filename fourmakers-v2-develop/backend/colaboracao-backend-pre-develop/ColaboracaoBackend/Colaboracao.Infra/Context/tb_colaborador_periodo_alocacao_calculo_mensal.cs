using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_periodo_alocacao_calculo_mensal
    {
        public Guid id { get; set; }
        public string codigo_colaborador { get; set; }
        public int? cod_tbd_alocado { get; set; }
        public int mes { get; set; }
        public int ano { get; set; }
        public decimal horas { get; set; }
        public string status_colaborador_periodo_alocacao { get; set; }
        public DateTime? data_criacao { get; set; }
        public int tb_org_id { get; set; }

        public virtual tb_org tb_org { get; set; }
    }
}