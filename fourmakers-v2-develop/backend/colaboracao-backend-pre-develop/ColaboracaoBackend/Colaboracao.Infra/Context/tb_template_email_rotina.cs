using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_template_email_rotina
    {
        public Guid id { get; set; }
        public string emails { get; set; }
        public int tb_org_id { get; set; }
        public string corpo_email_parametrizado { get; set; }
        public int numero_tentativas { get; set; }
        public string mensagem_erro { get; set; }
        public DateTime data_criacao { get; set; }
        public DateTime? data_disparo { get; set; }
        public DateTime? data_alteracao { get; set; }

        public virtual tb_org tb_org { get; set; }
    }
}