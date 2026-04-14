using System;

namespace DataTransferObject.Domain.RotinaNotificacaoContratosVencidos
{
    public class ContratoNotificacaoEmailRow
    {
        public string Email { get; set; }
        public string ContratoId { get; set; }
        public string NomeEmpresa { get; set; }
        public string NomeContrato { get; set; }
        public DateTime? FimContrato { get; set; }
    }
}
