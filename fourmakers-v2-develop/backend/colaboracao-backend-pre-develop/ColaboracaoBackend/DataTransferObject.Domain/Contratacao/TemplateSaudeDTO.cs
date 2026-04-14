using System.Text.Json.Serialization;
using DataTransferObject.Domain.Colaborador;


namespace DataTransferObject.Domain.Contratacao;

public class TemplateSaudeDTO
{
    [JsonPropertyName("pcd")]
    public string Pcd { get; set; } = "Não";
    
    [JsonPropertyName("tipoPcd")]
    public string TipoPcd { get; set; } = DataTransferObject.Domain.Colaborador.EnumPCD.Nenhuma.ToString();

    [JsonPropertyName("enumPCD")]
    public int EnumPCD { get; set; } = (int)DataTransferObject.Domain.Colaborador.EnumPCD.Nenhuma;

    [JsonPropertyName("grupoDeRiscoCovid")]
    public sbyte GrupoDeRiscoCovid { get; set; }

    [JsonPropertyName("condicaoDeSaudeRelevante")]
    public string CondicaoDeSaudeRelevante { get; set; } = string.Empty;
}

