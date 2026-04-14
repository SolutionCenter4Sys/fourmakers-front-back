using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace UploadFiles.API.DTOs
{
    public class RenomearArquivosRequest
    {
        [JsonPropertyName("renomes")]
        public List<RenomearItem> Renomes { get; set; } = new();
    }

    public class RenomearItem
    {
        [JsonPropertyName("nomeAtual")]
        public string NomeAtual { get; set; } = string.Empty;

        [JsonPropertyName("nomeNovo")]
        public string NomeNovo { get; set; } = string.Empty;
    }
}
