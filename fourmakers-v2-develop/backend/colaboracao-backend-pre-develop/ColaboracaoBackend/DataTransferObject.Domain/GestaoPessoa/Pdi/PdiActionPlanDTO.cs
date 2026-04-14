using System;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    public class PdiActionPlanDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? ConcluidoEm { get; set; }
        public string CodigoInternoColaboradorCriacao { get; set; }
        public string CodigoInternoColaboradorAlteracao { get; set; }
    }
}
