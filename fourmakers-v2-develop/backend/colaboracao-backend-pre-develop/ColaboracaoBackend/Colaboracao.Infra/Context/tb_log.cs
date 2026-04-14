using System;

#nullable disable

namespace Colaboracao.Infra.Context
{
    public partial class tb_log
    {
        public long id { get; set; }
        public string message { get; set; }
        public string complete_message { get; set; }
        public string stacktrace { get; set; }
        public string codigo_interno_colaborador_origin { get; set; }
        public DateTime date { get; set; }
        public string log_type { get; set; }
        public string process_identifier { get; set; }
    }
}