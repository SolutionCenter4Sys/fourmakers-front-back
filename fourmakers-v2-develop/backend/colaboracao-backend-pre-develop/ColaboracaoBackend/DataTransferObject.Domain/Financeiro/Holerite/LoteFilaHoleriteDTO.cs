using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Holerite
{
    public class LoteFilaResult : LoteFilaHoleriteDTO
    {
        
        public string Status { get; set; }

    }

    public class LoteFilaHoleriteDTO : Apontamento.FolhaPonto.LoteFilaDTO
    {
        /// <summary>
        /// Sumário da folha de ponto
        /// </summary>
        [JsonIgnore]
        public string SumarioHoleriteString { get; set; }
        public SumarioHoleriteDTO SumarioHolerite { get; set; }
    }
   
}


