using System;

namespace DataTransferObject.Domain.Log
{
    public class LogDTO
    {
        public DateTime Data { get; set; }
        public string Mensagem { get; set; }
        public string MensagemCompleta { get; set; }
        public string StackTrace { get; set; }
        public string Tipo { get; set; }
        public string Identificador { get; set; }
        public string CodigoColaboradorInternoOrigem { get; set; }
    }
}
