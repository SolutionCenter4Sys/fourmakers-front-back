using System;
using System.Collections.Generic;
using DataTransferObject.Domain.Arquivo;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;

public class SolicitacaoReembolsoDTO
{
    public string? ProjetoId { get; set; }
    public string? ClienteId { get; set; }
    public int VerbaId { get; set; }
    public string Descricao { get; set; } = null;
    public DateTime DataDespesa { get; set; }
    public decimal Valor { get; set; }
    public decimal? ValorUnidade { get; set; } = null;
    public int? Quantidade { get; set; } = null;

    public List<Base64DTO> Arquivos { get; set; } = new List<Base64DTO>();
}