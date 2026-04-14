using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.IntegracaoContabil;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;
using Microsoft.AspNetCore.Mvc;

namespace Financeiro.Domain.Interfaces.Reembolso.Solicitacao;

public interface ISolicitacaoExternoService
{
    Task<ApiGenericResult<List<RemessaContabilRegistroReembolsoDTO>>> ProcessarReembolsoAsync(string tokenSistema, bool atualizarStatusParaPago, string cnpj, string codigoColaboradorExternoAprovador);
}