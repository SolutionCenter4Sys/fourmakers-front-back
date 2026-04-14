using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria;

public class SolicitacaoRemessaDTO
{
    public string Id { get; set; }
    public string CodigoColaborador { get; set; }
    public string Nome { get; set; }
    public string Projeto { get; set; }
    public string Cliente { get; set; }
    public decimal ValorParaPagamento { get; set; }
    public DateTime DataSolicitacao { get; set; }
    [JsonIgnore]
    public string CodDiretoria  { get; set; }
}