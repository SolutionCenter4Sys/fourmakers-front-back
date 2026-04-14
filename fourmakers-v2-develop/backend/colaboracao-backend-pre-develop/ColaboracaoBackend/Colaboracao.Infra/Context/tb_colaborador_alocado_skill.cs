using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_alocado_skill
    {
        public long tb_colaborador_periodo_alocacao_id { get; set; }
        public long tb_item_perfil_id { get; set; }
        public long skill_id { get; set; }
        public long? tb_nivel_id { get; set; }
        public DateTime? data_criacao { get; set; }
        public Guid? codigo_interno_colaborador_criacao { get; set; }

        public virtual tb_colaborador_periodo_alocacao tb_colaborador_periodo_alocacao { get; set; }
    }
}