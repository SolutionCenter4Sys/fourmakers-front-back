using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaCarga;

public class FilaMessageRubricaCargaDTO
{
    [JsonPropertyName("rubricaId")]
    public string RubricaId { get; set; }
    
    [JsonPropertyName("mesInicial")]
    public int MesInicial { get; set; }
    
    [JsonPropertyName("anoFinal")]
    public int AnoFinal { get; set; }
    
    [JsonPropertyName("codDiretoria")]
    public string CodDiretoria  { get; set; }
    
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
    
    [JsonPropertyName("isLastPage")]
    public bool IsLastPage { get; set; }
    
    [JsonPropertyName("isFirstPage")]
    public bool IsFirstPage { get; set; }
    
    [JsonPropertyName("rubricaTemplate")]
    public TemplateRubricaDTO RubricaTemplate { get; set; }
    
    [JsonPropertyName("rubricaCargaLogId")]
    public string  RubricaCargaLogId { get; set; }
    
}