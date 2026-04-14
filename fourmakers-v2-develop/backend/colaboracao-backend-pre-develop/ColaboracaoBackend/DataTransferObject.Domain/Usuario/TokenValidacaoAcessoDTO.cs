using System;

namespace DataTransferObject.Domain
{
    public class TokenValidacaoAcessoDTO
    {
        public string Token { get; set; }
        public DateTime Validade { get; set; }
        public TipoTokenAcessoEnum Tipo { get; set; }
    }
}