#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_ad_sso
    {
        public int id { get; set; }
        public string base_url { get; set; }
        public string token_path { get; set; }
        public string graph_path { get; set; }
        public string tenant { get; set; }
        public string client_id { get; set; }
        public string client_secret_value { get; set; }
        public string code_verifier_plain { get; set; }
        public string redirect_url { get; set; }
        public string scope { get; set; }
        public int tb_org_id { get; set; }
        public int is_public { get; set; }

        public virtual tb_org tb_org { get; set; }
    }
}