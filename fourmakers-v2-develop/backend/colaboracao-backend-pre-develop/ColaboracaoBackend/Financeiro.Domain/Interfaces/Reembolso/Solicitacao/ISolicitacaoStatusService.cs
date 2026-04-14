using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;

namespace Financeiro.Domain.Interfaces.Reembolso.Solicitacao;

public interface ISolicitacaoStatusService
{
    Task<ApiGenericResult<List<SolicitacaoStatusDTO>>> ListarAsync(int orgId, string cpfRequest);
}