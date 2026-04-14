using System;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    public class PdiMetricaExportLinhaDTO
    {
        public Guid PdiId { get; set; }
        public string Titulo { get; set; }
        public string ColaboradorId { get; set; }
        public string Status { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DeadLine { get; set; }
    }
}
