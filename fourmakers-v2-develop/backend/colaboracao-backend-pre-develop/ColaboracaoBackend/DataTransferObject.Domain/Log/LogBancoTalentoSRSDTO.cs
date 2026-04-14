using System;

namespace DataTransferObject.Domain.Log
{
    public class LogBancoTalentoSRSDTO
    {
        public string Id { get; set; }
        public string UrlLinkedin { get; set; }
        public bool Sucesso { get; set; }
        public string MensagemRetorno { get; set; }
        public string StackTrace { get; set; }
        public DateTime Data { get; set; }
    }
}
