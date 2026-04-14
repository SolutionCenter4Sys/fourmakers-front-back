using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;

public class InserirSolicitacaoReembolsoListaDTO
{
    public string Objetivo { get; set; } = string.Empty;
    public string? Destino { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public List<SolicitacaoReembolsoDTO> Solicitacoes { get; set; }
}