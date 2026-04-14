using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using DataTransferObject.Domain.Financeiro.NotaFiscal;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;

namespace DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaboradorLiberacao;

public class RubricaColaboradorLiberacaoNfDTO 
{
    public Guid Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    [JsonIgnore]
    public string CodigoInternoColaborador { get; set; } = string.Empty;
    [JsonIgnore]
    public string NomeColaborador { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public bool EmUso { get; set; }
    [JsonIgnore]
    public Guid RubricaColaboradorId { get; set; }
    [JsonIgnore]
    public int VigenciaMes { get; set; }
    [JsonIgnore]
    public int VigenciaAno { get; set; }
    public string Natureza { get; set; }
    public List<NotaFiscalInfoDTO> NotasFiscais { get; set; }
    
}