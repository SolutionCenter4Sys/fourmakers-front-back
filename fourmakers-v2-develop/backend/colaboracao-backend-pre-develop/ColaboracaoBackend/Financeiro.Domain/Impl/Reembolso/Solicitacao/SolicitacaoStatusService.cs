using Colaboracao.Helper.Enum;
using Core.Domain.Reembolso.Solicitacao;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Solicitacao;
using DataTransferObject.Domain.Fourmakers;
using Financeiro.Domain.Interfaces.Reembolso.Solicitacao;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Attributes;

namespace Financeiro.Domain.Services.Reembolso.Solicitacao;

[LogDomainClass]
public class SolicitacaoStatusService(ISolicitacaoStatusRepository solicitacaoStatusRepository, IBuscaParametroConfiguracaoService _buscaParametroConfiguracaoService) : ISolicitacaoStatusService
{
    public async Task<ApiGenericResult<List<SolicitacaoStatusDTO>>> ListarAsync(int orgId, string cpfRequest)
    {
        var apiResult = new ApiGenericResult<List<SolicitacaoStatusDTO>>();
        try
        {
            var temCFO = _buscaParametroConfiguracaoService.GetParametroConfiguracao<bool>(ParametroOrgCodigoFrontEndEnum.REEMSOLBO_WORKFLOW_APROVACAO_CFO, orgId, cpfRequest);
            var retorno = await solicitacaoStatusRepository.ListarAsync();

            apiResult.Retorno = temCFO 
                ? retorno 
                : retorno.Where(x => x.Id != 5).ToList();
        }
        catch (Exception e)
        {
            ExceptionUtil.GerenciarRetornoExcecao(e, CRUDEnum.Read, "Status");
        }
        return apiResult;
    }
}