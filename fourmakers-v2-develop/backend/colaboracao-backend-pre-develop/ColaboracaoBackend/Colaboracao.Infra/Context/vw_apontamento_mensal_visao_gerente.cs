#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class vw_apontamento_mensal_visao_gerente
    {
        public string cod_projeto { get; set; }
        public string nome_projeto { get; set; }
        public string gerente { get; set; }
        public string tipo_gerente { get; set; }
        public string codigo_interno_colaborador_gerente { get; set; }
        public string cod_gerente { get; set; }
        public decimal? total_horas { get; set; }
        public int? mes { get; set; }
        public int? ano { get; set; }
        public int apontamento_reprovado { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public int? tb_org_id { get; set; }
        public int cod_status_mensal { get; set; }
        public string descricao_status_mensal { get; set; }
        public int? status_apontamento_grupo_prioridade { get; set; }
    }
}