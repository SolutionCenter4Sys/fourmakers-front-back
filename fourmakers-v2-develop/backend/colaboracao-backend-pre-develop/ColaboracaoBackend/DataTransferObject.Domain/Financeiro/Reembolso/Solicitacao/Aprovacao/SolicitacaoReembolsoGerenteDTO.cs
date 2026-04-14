using System;
using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

public class SolicitacaoReembolsoGerenteDTO
{
    public int Id { get; set; }
    public string CodigoInternoColaborador { get; set; }
    public string Colaborador { get; set; }
    public string ClienteDescricao { get; set; }
    public string ProjetoDescricao  { get; set; }
    public string Observacao { get; set; } = null;
    public string Valor { get; set; }
    public string Status { get; set; }
    public string Objetivo { get; set; }
    public string? Destino { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public DateTime DataSolicitacao { get; set; }
    public List<SolicitacaoDocumentoDTO> SolicitacaoDocumentos { get; set; } = [];
}