#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class vw_colaboradores_gestor
    {
        public string codigo_interno_colaborador { get; set; }
        public string nome_completo_colaborador { get; set; }
        public string nome_completo { get; set; }
        public int tb_org_id { get; set; }
        public string cod_gerente { get; set; }
        public string cod_colaborador { get; set; }
        public string nome_completo_gerente { get; set; }
    }
}