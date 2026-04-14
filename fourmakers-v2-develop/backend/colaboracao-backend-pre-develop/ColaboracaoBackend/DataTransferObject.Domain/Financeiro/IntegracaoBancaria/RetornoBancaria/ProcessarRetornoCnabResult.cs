using System;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RetornoBancaria
{
    public class ProcessarRetornoCnabResult
    {
        public Guid RetornoId { get; set; }
        public int TotalLinhasProcessadas { get; set; }
        public int PagamentosAtualizados { get; set; }
        public string Mensagem { get; set; }
    }
}
