using System.Collections.Generic;

namespace DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao.Aprovacao;

public class AprovacaoSolicitacaoDTO
{
    public List<int> SolicitacoesIds { get; set; }
    public string Observacao { get; set; } = string.Empty;
}