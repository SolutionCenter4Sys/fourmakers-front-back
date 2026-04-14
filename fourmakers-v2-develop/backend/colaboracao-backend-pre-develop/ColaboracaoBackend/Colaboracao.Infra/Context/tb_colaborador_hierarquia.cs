#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_hierarquia
    {
        public string cod_colaborador_externo { get; set; }
        public string cod_colaborador_superior { get; set; }
        public int tb_org_id { get; set; }

        public virtual tb_org tb_org { get; set; }
    }
}