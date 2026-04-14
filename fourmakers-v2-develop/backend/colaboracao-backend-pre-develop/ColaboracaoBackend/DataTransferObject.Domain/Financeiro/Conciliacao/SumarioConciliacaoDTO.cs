using System;

namespace DataTransferObject.Domain.Financeiro.Conciliacao
{
    public class SumarioConciliacaoResult : SumarioConciliacaoDTO
    {   
        public string Id { get; set; }
        public int QuantidadeItens { get; set; }
        public string Status { get; set; } = "Em processamento";
        public string StatusConciliacao { get; set; }
        public StatusConciliacaoEnum StatusCode { get; set; }
        public int QuantidadeItensProcessados { get; set; } = 0;
        public int QuantidadeItensProcessadosComErro { get; set; } = 0;
    }
    public class SumarioConciliacaoDTO
    {
        public string Cnpj { get; set; }
        public string Empresa { get; set; }
        public string Competencia { get; set; }
    }
}


