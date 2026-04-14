namespace DataTransferObject.Domain.Financeiro.NotaFiscal;

using System;
using System.Text.Json.Serialization;

public class AnaliseIANotaFiscalValorDTO
{
    /// <summary>
    /// Valor total da nota fiscal
    /// </summary>
    [JsonPropertyName("valor")]
    public decimal? Valor { get; set; }
    
    /// <summary>
    /// Moeda utilizada (ex: BRL, USD)
    /// </summary>
    [JsonPropertyName("moeda")]
    public string Moeda { get; set; }
    
    /// <summary>
    /// Data de emissão da nota fiscal no formato YYYY-MM-DD
    /// </summary>
    [JsonPropertyName("dataEmissao")]
    public DateTime? DataEmissao { get; set; }
    
    /// <summary>
    /// Número da nota fiscal
    /// </summary>
    [JsonPropertyName("numero")]
    public string Numero { get; set; }
    
    /// <summary>
    /// Série da nota fiscal (opcional)
    /// </summary>
    [JsonPropertyName("serie")]
    public string Serie { get; set; }
    
    /// <summary>
    /// CNPJ do emitente sem pontuação (opcional)
    /// </summary>
    [JsonPropertyName("cnpjEmitente")]
    public string CnpjEmitente { get; set; }
    
    /// <summary>
    /// Nome da empresa emitente (opcional)
    /// </summary>
    [JsonPropertyName("nomeEmitente")]
    public string NomeEmitente { get; set; }
}