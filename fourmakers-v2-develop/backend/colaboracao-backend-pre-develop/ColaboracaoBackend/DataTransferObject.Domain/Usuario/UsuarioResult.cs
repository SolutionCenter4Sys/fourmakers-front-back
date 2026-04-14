using DataTransferObject.Domain.Base;
using System;

namespace DataTransferObject.Domain.Usuario
{
    public class UsuarioResult : StatusResult
    {
        public UsuarioColaboradorDTO usuario { get; set; }
        public bool primeiroAcessoRealizado { get; set; }
        public DateTime? dataAceitePrimeiroAcesso { get; set; }
        public string token { get; set; }
    }
}