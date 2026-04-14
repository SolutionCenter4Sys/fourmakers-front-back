using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DataTransferObject.Domain.Apontamento.FolhaPonto;

namespace DataTransferObject.Domain.Financeiro.Holerite
{
    public class DetalhesLoteHoleriteResult 
    {
        [JsonPropertyName("lote")]
        public SumarioHoleriteResult Lote { get; set; }
        [JsonPropertyName("itens")]
        public List<ItemLoteColaboradorDTO> Itens { get; set; }
    }
}