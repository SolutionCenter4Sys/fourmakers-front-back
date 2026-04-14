using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.GestaoPessoa.Pdi
{
    /// <summary>Conteúdo de uma evidência para download (ler arquivo) ou apenas URL externa.</summary>
    public class PdiEvidenciaDownloadDTO
    {
        public byte[] Content { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

        [JsonPropertyName("link")]
        public string Link { get; set; }
    }
}
