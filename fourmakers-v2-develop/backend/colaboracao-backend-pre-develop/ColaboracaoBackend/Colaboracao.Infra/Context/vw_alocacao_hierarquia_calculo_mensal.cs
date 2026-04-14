#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class vw_alocacao_hierarquia_calculo_mensal
    {
        public string cod_colaborador_externo { get; set; }
        public string cod_colaborador_superior { get; set; }
        public long eh_tbd { get; set; }
        public int? tb_org_id { get; set; }
    }
}