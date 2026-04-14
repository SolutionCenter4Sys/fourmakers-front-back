using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_notificacao
    {
        public Guid id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public string titulo { get; set; }
        public string mensagem { get; set; }
        public string mensagem_html { get; set; }
        public bool? lida { get; set; }
        public DateTime? data_envio { get; set; }
        public DateTime? data_leitura { get; set; }
        public int? tb_funcionalidade_sistema_id { get; set; }
        public string url_customizada { get; set; }
        public int tb_org_id { get; set; }

        public virtual tb_colaborador codigo_interno_colaboradorNavigation { get; set; }
        public virtual tb_funcionalidade_sistema tb_funcionalidade_sistema { get; set; }
        public virtual tb_org tb_org { get; set; }
    }
}