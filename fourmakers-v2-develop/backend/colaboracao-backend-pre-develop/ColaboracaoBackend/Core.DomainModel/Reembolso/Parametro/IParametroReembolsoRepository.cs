using System.Threading.Tasks;
using DataTransferObject.Domain.Financeiro.Reembolso.Parametro;

namespace Core.Domain.Reembolso.Parametro;

public interface IParametroReembolsoRepository
{
    Task<ParametroReembolsoDTO> InserirParametroAsync(int limiteEnvio, int diaPagamento, int validadeComprovanteDias, int orgId, int? limiteEnvioAlternativo, int? diaPagamentoAlternativo, string codigoInternoColaborador, bool permitirAprovarMinhasSolicitacoes);
    Task<ParametroReembolsoDTO> EditarAsync(int id, int limiteEnvio, int diaPagamento, int validadeComprovanteDias, int? limiteEnvioAlternativo, int? diaPagamentoAlternativo, string codigoInternoColaborador, bool permitirAprovarMinhasSolicitacoes);
    Task<ParametroReembolsoDTO> BuscarPorIdAsync(int id);
    Task<ParametroReembolsoDTO?> BuscarPorOrgIdAsync(int orgId);
}