using System.Collections.Generic;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_org
    {
        public tb_org()
        {
            tb_ad_sso = new HashSet<tb_ad_sso>();
            tb_apontamento_periodo_fechado = new HashSet<tb_apontamento_periodo_fechado>();
            tb_apontamento_periodo_fechado_log = new HashSet<tb_apontamento_periodo_fechado_log>();
            tb_atividade = new HashSet<tb_atividade>();
            tb_cliente_org = new HashSet<tb_cliente_org>();
            tb_colaborador_alocado = new HashSet<tb_colaborador_alocado>();
            tb_colaborador_hierarquia = new HashSet<tb_colaborador_hierarquia>();
            tb_colaborador_org = new HashSet<tb_colaborador_org>();
            tb_colaborador_periodo_alocacao = new HashSet<tb_colaborador_periodo_alocacao>();
            tb_colaborador_periodo_alocacao_calculo_mensal = new HashSet<tb_colaborador_periodo_alocacao_calculo_mensal>();
            tb_colaborador_projeto_org = new HashSet<tb_colaborador_projeto_org>();
            tb_empresa = new HashSet<tb_empresa>();
            tb_feriado = new HashSet<tb_feriado>();
            tb_funcionalidade_rota = new HashSet<tb_funcionalidade_rota>();
            tb_grupo_acesso = new HashSet<tb_grupo_acesso>();
            tb_notificacao = new HashSet<tb_notificacao>();
            tb_parametro_configuracao = new HashSet<tb_parametro_configuracao>();
            tb_projeto_gerente = new HashSet<tb_projeto_gerente>();
            tb_projeto_org = new HashSet<tb_projeto_org>();
            tb_status_projeto = new HashSet<tb_status_projeto>();
            tb_template_email_rotina = new HashSet<tb_template_email_rotina>();
            tb_token_sistema = new HashSet<tb_token_sistema>();
        }

        public int id { get; set; }
        public string descricao { get; set; }
        public string subdominio { get; set; }
        public string dominio_email { get; set; }

        public virtual ICollection<tb_ad_sso> tb_ad_sso { get; set; }
        public virtual ICollection<tb_apontamento_periodo_fechado> tb_apontamento_periodo_fechado { get; set; }
        public virtual ICollection<tb_apontamento_periodo_fechado_log> tb_apontamento_periodo_fechado_log { get; set; }
        public virtual ICollection<tb_atividade> tb_atividade { get; set; }
        public virtual ICollection<tb_cliente_org> tb_cliente_org { get; set; }
        public virtual ICollection<tb_colaborador_alocado> tb_colaborador_alocado { get; set; }
        public virtual ICollection<tb_colaborador_hierarquia> tb_colaborador_hierarquia { get; set; }
        public virtual ICollection<tb_colaborador_org> tb_colaborador_org { get; set; }
        public virtual ICollection<tb_colaborador_periodo_alocacao> tb_colaborador_periodo_alocacao { get; set; }
        public virtual ICollection<tb_colaborador_periodo_alocacao_calculo_mensal> tb_colaborador_periodo_alocacao_calculo_mensal { get; set; }
        public virtual ICollection<tb_colaborador_projeto_org> tb_colaborador_projeto_org { get; set; }
        public virtual ICollection<tb_empresa> tb_empresa { get; set; }
        public virtual ICollection<tb_feriado> tb_feriado { get; set; }
        public virtual ICollection<tb_funcionalidade_rota> tb_funcionalidade_rota { get; set; }
        public virtual ICollection<tb_grupo_acesso> tb_grupo_acesso { get; set; }
        public virtual ICollection<tb_notificacao> tb_notificacao { get; set; }
        public virtual ICollection<tb_parametro_configuracao> tb_parametro_configuracao { get; set; }
        public virtual ICollection<tb_projeto_gerente> tb_projeto_gerente { get; set; }
        public virtual ICollection<tb_projeto_org> tb_projeto_org { get; set; }
        public virtual ICollection<tb_status_projeto> tb_status_projeto { get; set; }
        public virtual ICollection<tb_template_email_rotina> tb_template_email_rotina { get; set; }
        public virtual ICollection<tb_token_sistema> tb_token_sistema { get; set; }
    }
}