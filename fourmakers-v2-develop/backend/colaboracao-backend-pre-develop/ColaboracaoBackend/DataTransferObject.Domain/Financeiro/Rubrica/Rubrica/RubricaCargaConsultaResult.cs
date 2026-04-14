using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;

public class RubricaCargaConsultaResult : RubricaCargaBase
{
    [JsonPropertyName("data_criacao")]
    public DateTime DataCriacao { get; set; }

    [JsonPropertyName("data_processamento_fim")]
    public DateTime DataProcessamentoFim { get; set; }

    [JsonPropertyName("mensagem_erro")]
    public string? MensagemErro { get; set; }

    [JsonPropertyName("qtd_registros_processados_sucesso")]
    public int QtdRegistrosProcessadosSucesso { get; set; }

    [JsonPropertyName("qtd_registros_retornados")]
    public int QtdRegistrosRetornados { get; set; }
}