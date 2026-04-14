using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

public class SolicitacaoColaboradorAprovacaoDTO
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public string CategoriaDescricao { get; set; }
    public DateTime DataDespesa { get; set; }
    public decimal Valor { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public int StatusId { get; set; }
    public string StatusDescricao { get; set;  }
    public List<SolicitacaoDocumentoDTO> SolicitacaoDocumentos { get; set; } = [];
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public string Destino  { get; set; } = string.Empty;
    public string ClienteDescricao  { get; set; } = string.Empty;
    public string ProjetoDescricao { get; set; } =  string.Empty;
    public string Colaborador { get; set; } =  string.Empty;
    public string Objetivo { get; set; } =  string.Empty;
}