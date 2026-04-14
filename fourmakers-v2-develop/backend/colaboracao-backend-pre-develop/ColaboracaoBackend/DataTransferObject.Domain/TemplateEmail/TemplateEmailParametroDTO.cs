using System;

namespace DataTransferObject.Domain.TemplateEmail
{
    public class TemplateEmailParametroDTO
    {
        public Guid id { get; set; }
        public string parametro { get; set; }
        public string valor_fixo { get; set; }
        public string tabela { get; set; }
        public string coluna { get; set; }
        public string condicao { get; set; }
        public string codigo_fonte_dados { get; set; }
        public sbyte? ativo { get; set; }
        public DateTime? data_alteracao { get; set; }
        public DateTime? data_criacao { get; set; }
        public Guid tb_template_email_id { get; set; }
    }
}