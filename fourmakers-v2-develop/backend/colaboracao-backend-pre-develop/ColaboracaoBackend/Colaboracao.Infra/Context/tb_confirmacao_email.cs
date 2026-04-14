using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_confirmacao_email
    {
        public int id { get; set; }
        public string nome_colaborador { get; set; }
        public long? usuario_id { get; set; }
        public string codigo_interno_colaborador { get; set; }
        public string email { get; set; }
        public int? codigo_enviado { get; set; }
        public bool? codigo_confirmado { get; set; }
        public DateTime? hora_expiracao_codigo { get; set; }
    }
}