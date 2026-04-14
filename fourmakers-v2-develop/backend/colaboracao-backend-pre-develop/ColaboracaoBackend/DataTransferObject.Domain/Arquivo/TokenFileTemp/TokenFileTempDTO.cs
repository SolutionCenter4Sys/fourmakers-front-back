using System;

namespace DataTransferObject.Domain.Arquivo.TokenFileTemp
{
    public class TokenFileTempDTO
    {
        public string Token { get; set; }
        public string NomeArquivo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
