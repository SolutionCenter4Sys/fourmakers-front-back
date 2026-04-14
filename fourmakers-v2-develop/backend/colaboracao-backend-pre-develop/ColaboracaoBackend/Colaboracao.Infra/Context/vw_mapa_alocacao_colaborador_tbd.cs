#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class vw_mapa_alocacao_colaborador_tbd
    {
        public string cod_profisisonal { get; set; }
        public string nome_profissional { get; set; }
        public string codigo_interno_colaborador_gestor { get; set; }
        public string codigo_diretoria { get; set; }
        public string codigo_departamento { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long eh_tbd { get; set; }
        public int? tb_org_id { get; set; }
    }
}