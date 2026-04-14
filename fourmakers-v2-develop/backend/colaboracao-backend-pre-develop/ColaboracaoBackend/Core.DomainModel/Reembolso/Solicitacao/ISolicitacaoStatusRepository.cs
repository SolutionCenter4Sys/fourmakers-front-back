using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;

namespace Core.Domain.Reembolso.Solicitacao;

public interface ISolicitacaoStatusRepository
{
    Task<List<SolicitacaoStatusDTO>> ListarAsync();
}