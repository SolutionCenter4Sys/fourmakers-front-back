using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Conciliacao
{
    public class ConciliacaoLoteResult {
        public SumarioConciliacaoResult SumarioConciliacao { get; set; }
        public List<ItemConciliacaoColaboradorDTO> ItensConciliacao { get; set; }
    }
    public class ItemConciliacaoColaboradorDTO
    {
        public string NomeColaborador { get; set; }
        public string CodigoInternoColaborador { get; set; }
        public string Cargo { get; set; }
        public string HoleritePath { get; set; }
        public string FolhaPontoPath { get; set; }
        public ConciliacaoFolhaPontoColaboradorDTO ResultadoConciliacao { get; set; }
    }
} 