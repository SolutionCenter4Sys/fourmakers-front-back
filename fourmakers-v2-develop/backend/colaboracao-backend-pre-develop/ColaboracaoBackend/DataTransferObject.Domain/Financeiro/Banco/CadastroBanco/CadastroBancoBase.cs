using System;

namespace DataTransferObject.Domain.Financeiro.Banco.CadastroBanco
{
    public class CadastroBancoBase
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string CodigoBanco { get; set; }
    }
}