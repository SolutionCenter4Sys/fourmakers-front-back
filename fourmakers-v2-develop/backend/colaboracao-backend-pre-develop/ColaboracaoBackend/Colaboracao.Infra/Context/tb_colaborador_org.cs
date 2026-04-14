using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_org
    {
        public tb_colaborador_org()
        {
            tb_colaborador_apontamento = new HashSet<tb_colaborador_apontamento>();
            tb_colaborador_apontamento_log = new HashSet<tb_colaborador_apontamento_log>();
            tb_parametro_configuracao = new HashSet<tb_parametro_configuracao>();
        }

        public int tb_org_id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public string cod_diretoria { get; set; }
        public string diretoria { get; set; }
        public string departamento { get; set; }
        public string cod_departamento { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string cargo { get; set; }
        public string codigo_cargo { get; set; }
        public string cod_colaborador_externo { get; set; }
        public DateTime? data_admissao { get; set; }
        public sbyte ativo { get; set; }
        public DateTime? data_inativacao { get; set; }
        public string modelo_contratacao { get; set; }
        public string empresa_relacionada { get; set; }
        public string modelo_trabalho { get; set; }
        public int? dias_por_semana { get; set; }
        public decimal? valor_hora { get; set; }
        public decimal? custo_hora { get; set; }
        public int? base_hora_mes { get; set; }
        public string? codigo_modelo_contratacao { get; set; }

        public string idioma { get; set; }
        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_org tb_org { get; set; }
        public virtual ICollection<tb_colaborador_apontamento> tb_colaborador_apontamento { get; set; }
        public virtual ICollection<tb_colaborador_apontamento_log> tb_colaborador_apontamento_log { get; set; }
        public virtual ICollection<tb_parametro_configuracao> tb_parametro_configuracao { get; set; }
    }
}