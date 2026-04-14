using System;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    public class PdiConcluirActionPlanResponseDTO
    {
        public Guid Id { get; set; }
        public DateTime? ConcluidoEm { get; set; }
        public double Progress { get; set; }
        public string Status { get; set; }
    }
}
