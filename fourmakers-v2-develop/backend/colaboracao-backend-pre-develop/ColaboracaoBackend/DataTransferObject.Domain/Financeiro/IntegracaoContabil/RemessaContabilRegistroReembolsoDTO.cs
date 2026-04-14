using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.IntegracaoContabil
{
    public class RemessaContabilRegistroReembolsoDTO : RemessaContabilRegistroDTO
    {
        public int ReembolsoId { get; set; }
        public DateTime? DataDespesa { get; set; }
        public DateTime? DataSolicitacao { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public string? FormaPagamento { get; set; }
        public List<string> Documentos { get; set; }
        [JsonIgnore]
        public string DocumentosJson { get; set; }

    }

}
