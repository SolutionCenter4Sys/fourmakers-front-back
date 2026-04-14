using System;
using System.Text.Json.Serialization;

namespace DataTransferObject.Domain.Financeiro.Banco.DadosBancariosColaborador;

public class DadosBancariosColaboradorResult : DadosBancariosColaboradorBase
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public DateTime DataCriacao { get; set; }
    [JsonIgnore]
    public DateTime DataAtualizacao { get; set; }
    [JsonIgnore]
    public string CodigoInternoColaborador { get; set; }
}