using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Reembolso.Verba;

namespace Financeiro.Domain.Interfaces.Reembolso.Verba;

public interface IVerbaService
{
    Task<ApiGenericResult<VerbaEParametroDTO>> EditarAsync(VerbaEParametroDTO input, int orgId, string codigoInternoColaborador);
    Task<ApiGenericResult<VerbaEParametroDTO>> ObterVerbasEParametro(int orgId, string cpf);
    Task<ApiGenericResult<List<VerbaSimplificadoDTO>>> ListarSimplificadoAsync(int orgId);
    Task<ApiGenericResult<VerbaEParametroValidacaoDTO>> ObterVerbasEParametroValidacao(int orgId, string cpf);
    Task<ApiGenericResult<List<VerbaSimplificadoDTO>>> ListarSimplificadoComExcecaoAsync(int orgId, string cpfRequset, string codigoProjeto, string codigoCliente);
}