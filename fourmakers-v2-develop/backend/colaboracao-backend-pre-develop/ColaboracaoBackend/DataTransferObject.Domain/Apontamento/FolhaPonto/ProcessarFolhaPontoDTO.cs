using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento.FolhaPonto
{

    public class SumarioFolhaPontoResult : SumarioFolhaPontoDTO
    {
        public string Id { get; set; }
    }

    public class SumarioFolhaPontoDTO
    {
        public string Cnpj { get; set; }
        public string Empresa { get; set; }
        public string Competencia { get; set; }
        public int QuantidadeItens { get; set; }
        public int QuantidadeItensProcessados { get; set; }
        public int QuantidadeItensProcessadosComErro { get; set; }
    }

    public class DetalhesLoteFolhaPontoResult 
    {
        [JsonPropertyName("lote")]
        public SumarioFolhaPontoResult Lote { get; set; }
        [JsonPropertyName("itens")]
        public List<ItemLoteColaboradorDTO> Itens { get; set; }
    }

} 