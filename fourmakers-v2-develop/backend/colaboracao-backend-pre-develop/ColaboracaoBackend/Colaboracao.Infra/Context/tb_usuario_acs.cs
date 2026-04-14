#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_usuario_acs
    {
        public string cloud_id { get; set; }
        public long usuario_id { get; set; }

        public virtual tb_usuario usuario { get; set; }
    }
}