#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class vw_alocacacao_recurso_calculo_mensal
    {
        public string codigo_colaborador { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public string nome { get; set; }
        public long ativo { get; set; }
        public int? tb_org_id { get; set; }
        public string cod_diretoria { get; set; }
        public string diretoria { get; set; }
        public long eh_tbd { get; set; }
        public string codigo_interno_colaborador_gestor { get; set; }
    }
}