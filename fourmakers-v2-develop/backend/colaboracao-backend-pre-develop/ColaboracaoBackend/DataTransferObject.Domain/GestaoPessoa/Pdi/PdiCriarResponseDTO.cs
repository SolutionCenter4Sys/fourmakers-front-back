using System;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    public class PdiCriarResponseDTO
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
