using System.Collections.Generic;

namespace DataTransferObject.Domain.Base
{
    public class StatusResult
    {
        public bool Sucesso { get; set; } = true;
        public string Mensagem { get; set; }
        public List<string> Erros { get; set; }
    }
}