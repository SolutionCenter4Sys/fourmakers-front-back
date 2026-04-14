using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Social
{
    /// <summary>
    /// Destinatário de um feedback 360 (tb_feedback360_destinatario + nome + fotos como no ShowMe).
    /// </summary>
    public class Feedback360DestinatarioItemDTO
    {
        public Guid CodigoInternoColaborador { get; set; }
        public string NomeCompleto { get; set; }

        [JsonPropertyName("urlFoto")]
        public string UrlFoto { get; set; }

        [JsonPropertyName("urlFotoThumb")]
        public string UrlFotoThumb { get; set; }

        [JsonPropertyName("urlFotoThumbMini")]
        public string UrlFotoThumbMini { get; set; }

        [JsonPropertyName("urlFotoThumbVeryMini")]
        public string UrlFotoThumbVeryMini { get; set; }
    }
}
