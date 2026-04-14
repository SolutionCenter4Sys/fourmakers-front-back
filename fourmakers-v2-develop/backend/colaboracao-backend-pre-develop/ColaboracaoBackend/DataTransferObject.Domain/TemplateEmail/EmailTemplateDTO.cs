using System;

namespace DataTransferObject.Domain.TemplateEmail
{
    public class EmailTemplateDTO
    {
        public Guid id { get; set; }
        public string codigo { get; set; }
        public string descricao { get; set; }
        public string template { get; set; }
        public int tb_org_id { get; set; }
        public sbyte? ativo { get; set; }
        public DateTime? data_alteracao { get; set; }
        public DateTime? data_criacao { get; set; }
    }
}