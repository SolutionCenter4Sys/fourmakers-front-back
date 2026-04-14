using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;

public class SolicitacaoReembolsoColaboradorDTO
{
    public int Id { get; set; }
    public string ClienteId { get; set; }
    public string ClienteDescricao  { get; set; }
    public string ProjetoId { get; set; }
    public string ProjetoDescricao  { get; set; }
    public string Categoria  { get; set; }
    public decimal ValorSolicitado { get; set; }
    public decimal? ValorAprovado { get; set; } = 0M;
    public DateTime? DataAprovacao { get; set; } = null;
    public DateTime Data { get; set; }
    public string Status { get; set; }
    public int StatusId { get; set; }
    public string Observacao { get; set; } 
    public string Objetivo { get; set; }
    public string? Destino { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public List<SolicitacaoDocumentoDTO> SolicitacaoDocumentos { get; set; }
}