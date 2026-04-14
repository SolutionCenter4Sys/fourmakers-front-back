using System;

namespace Colaboracao.Infra.Context
{
    public class tb_historico_cv
    {
        public string id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public long tb_item_perfil_id { get; set; }
        public string tb_origem_historico_cv_id { get; set; }
        public string tb_tipo_historico_cv_id { get; set; }
        public long? tb_skill_id { get; set; }
        public long? tb_nivel_id { get; set; }
        public DateTime data_criacao { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_item_perfil tb_item_perfil { get; set; }
        public virtual tb_nivel tb_nivel { get; set; }
        public virtual tb_origem_historico_cv tb_origem_historico_cv { get; set; }
        public virtual tb_tipo_historico_cv tb_tipo_historico_cv { get; set; }
    }
}