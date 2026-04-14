using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Conciliacao
{

    public class LoteFilaConciliacaoDTO : Apontamento.FolhaPonto.LoteFilaDTO
    {
        /// <summary>
        /// Sumário da folha de ponto
        /// </summary>
        [JsonIgnore]
        public string SumarioConciliacaoString { get; set; }
    }
   
}


