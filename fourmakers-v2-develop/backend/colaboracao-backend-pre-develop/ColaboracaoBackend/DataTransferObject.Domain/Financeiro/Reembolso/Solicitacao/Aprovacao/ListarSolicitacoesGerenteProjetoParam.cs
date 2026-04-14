using System;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

public class ListarSolicitacoesGerenteProjetoParam
{
    public string Filtro { get; set; }
    public string ClienteId { get; set; }
    public string ProjetoId { get; set; }
    public string DataInicio { get; set; } = string.Empty;
    public string DataFim { get; set; } = string.Empty;
    public int? StatusId { get; set; } = null;
}