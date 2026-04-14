#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class vw_gestores_colaboradores_org
    {
        public string cod_colaborador_externo_subordinado { get; set; }
        public string codigo_interno_colaborador_subordinado { get; set; }
        public string nome_completo_subordinado { get; set; }
        public string cod_colaborador_externo_gestor { get; set; }
        public string codigo_interno_colaborador_gestor { get; set; }
        public string nome_completo_gestor { get; set; }
        public int tb_org_id { get; set; }
    }
}