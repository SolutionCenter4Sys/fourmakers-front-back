#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_perfil_alocacao
    {
        public string id { get; set; }
        public int tb_org_id { get; set; }
        public long tb_colaborador_periodo_alocacao_id { get; set; }
        public string tb_gestor_externo_perfil_id { get; set; }
        public string tb_perfil_id { get; set; }

        public virtual tb_colaborador_periodo_alocacao tb_colaborador_periodo_alocacao { get; set; }
        public virtual tb_perfil tb_perfil { get; set; }
    }
}