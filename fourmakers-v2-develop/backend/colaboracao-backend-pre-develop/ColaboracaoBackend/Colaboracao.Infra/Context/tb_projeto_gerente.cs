#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_projeto_gerente
    {
        public string cod_projeto { get; set; }
        public string cod_colaborador_gerente { get; set; }
        public string tipo_gerente { get; set; }
        public int tb_org_id { get; set; }

        public virtual tb_org tb_org { get; set; }
    }
}