using System;

namespace DataTransferObject.Domain.TemplateEmail
{
    public class EmailDTO
    {
        public Guid Id { get; set; }
        public string Destinatarios { get; set; }
        public string Assunto { get; set; }
        public string CorpoEmail { get; set; }
        public int NumeroTentativas { get; set; }
        public string MensagemErro { get; set; }
        public DateTime? DataDisparo { get; set; }
        public DateTime? DataAlteracao { get; set; }
    }
}