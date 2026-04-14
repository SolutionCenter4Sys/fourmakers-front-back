using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Vaga
{
    public class AtualizarOrdemStatusVagaRecrutamentoParam
    {
        [Required]
        [JsonPropertyName("statusOrdem")]
        public List<StatusOrdemItem> StatusOrdem { get; set; }
    }

    public class StatusOrdemItem
    {
        [Required]
        [JsonPropertyName("codigo")]
        public int Codigo { get; set; }

        [Required]
        [JsonPropertyName("ordem")]
        public int Ordem { get; set; }
    }
}

