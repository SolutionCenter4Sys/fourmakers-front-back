using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Apontamento.FolhaPonto
{
    public class FilaMessageDTO
    {
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; }

        [JsonPropertyName("orgId")]
        public int OrgId { get; set; }

        [JsonPropertyName("loteId")]
        public string LoteId { get; set; }

        [JsonPropertyName("itemLoteId")]
        public string ItemLoteId { get; set; }

        [JsonPropertyName("usuarioId")]
        public long UsuarioId { get; set; }

        [JsonPropertyName("codigoColaboradorSolicitante")]
        public string CodigoColaboradorSolicitante { get; set; }

        [JsonPropertyName("pdfPath")]
        public string PdfPath { get; set; }

        [JsonPropertyName("tipoProcessamento")]
        public string TipoProcessamento { get; set; }
    }
} 