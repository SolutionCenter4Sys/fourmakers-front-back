using System;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RetornoBancaria
{
    public class PagamentoCnabOrdenadoDTO
    {
        public Guid Id { get; set; }
        public string HashPagamento { get; set; }
    }
}
