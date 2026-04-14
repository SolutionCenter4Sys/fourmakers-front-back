#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_status_projeto
    {
        public int id { get; set; }
        public string descricao_status { get; set; }
        public int cod_status { get; set; }
        public int tb_org_id { get; set; }
        public bool ativo { get; set; }

        public virtual tb_org tb_org { get; set; }
    }
}