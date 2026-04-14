using System;

namespace DataTransferObject.Domain.Financeiro.Holerite
{
    public class SumarioHoleriteResult : SumarioHoleriteDTO
    {   
        public string Id { get; set; }
        public int QuantidadeItens { get; set; }
        public int QuantidadeItensProcessados { get; set; }
        public int QuantidadeItensProcessadosComErro { get; set; }
    }
    public class SumarioHoleriteIaResult 
    {
        public SumarioHoleriteDTO Data { get; set; }
        public bool Success { get; set; }
    }
    public class SumarioHoleriteDTO
    {
        public string Cnpj { get; set; }
        public string Empresa { get; set; }
        public string Competencia { get; set; }
        public bool? Adiantamento { get; set; }
        public bool? Ferias { get; set; }
        public bool? DecimoTerceiro { get; set; }
        public bool? DecimoTerceiroAdiantamento { get; set; }
        public bool? InformeDeRendimentos { get; set; }
    }
}


