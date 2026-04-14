using System;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    public class PdiAprovarResponseDTO
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime AprovadoEm { get; set; }
    }
}
