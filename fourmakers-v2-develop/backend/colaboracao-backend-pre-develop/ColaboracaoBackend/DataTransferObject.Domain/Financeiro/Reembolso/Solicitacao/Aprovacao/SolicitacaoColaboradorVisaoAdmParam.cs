using System;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

public class SolicitacaoColaboradorVisaoAdmParam
{
    public string DataInicio { get; set; }
    public string DataFim  { get; set; }
    public string CodigoProjeto  { get; set; }
    public string CodigoCliente  { get; set; }
    public int? StatusId  { get; set; }
    public string AprovadorId  { get; set; }
}