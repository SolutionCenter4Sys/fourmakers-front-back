using System;
using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_colaborador_periodo_alocacao
    {
        public tb_colaborador_periodo_alocacao()
        {
            tb_colaborador_alocado_skill = new HashSet<tb_colaborador_alocado_skill>();
            tb_perfil_alocacao = new HashSet<tb_perfil_alocacao>();
        }
        public long id { get; set; }
        public DateTime data_inicio { get; set; }
        public DateTime data_fim { get; set; }
        public double quantidade_horas { get; set; }
        public sbyte retroalimenta_cv { get; set; }
        public sbyte inclui_fimdesemana { get; set; }
        public sbyte ativo { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime data_alteracao { get; set; }
        public string observacao { get; set; }
        public string oportunidade { get; set; }
        public sbyte? prioritario { get; set; }
        public double? percentual { get; set; }
        public string codigo_colaborador { get; set; }
        public string codigo_projeto { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public int tb_org_id { get; set; }
        public int? cod_tbd_alocado { get; set; }
        public Guid? tb_atividade_id { get; set; }

        public virtual tb_tbd_alocado cod_tbd_alocadoNavigation { get; set; }
        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_atividade tb_atividade { get; set; }
        public virtual tb_org tb_org { get; set; }

        public virtual ICollection<tb_colaborador_alocado_skill> tb_colaborador_alocado_skill { get; set; }
        public virtual ICollection<tb_perfil_alocacao> tb_perfil_alocacao { get; set; }
    }
}