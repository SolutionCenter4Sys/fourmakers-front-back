using System;

namespace DataTransferObject.Domain.TemplateEmail
{
    public class TemplateEmailDTO
    {
        public Guid Id { get; set; }
        public string Template { get; set; }
    }
}