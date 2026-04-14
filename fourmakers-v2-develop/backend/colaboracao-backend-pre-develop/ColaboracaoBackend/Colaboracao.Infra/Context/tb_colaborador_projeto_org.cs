#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_projeto_org
    {
        public string cod_colaborador { get; set; }
        public string cod_projeto { get; set; }
        public string nome_projeto { get; set; }
        public int tb_org_id { get; set; }

        public virtual tb_org tb_org { get; set; }
    }
}