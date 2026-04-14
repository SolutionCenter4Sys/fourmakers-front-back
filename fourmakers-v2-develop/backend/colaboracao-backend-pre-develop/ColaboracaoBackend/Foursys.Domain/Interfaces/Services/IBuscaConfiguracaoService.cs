using DataTransferObject.Domain.Fourmakers;

namespace Foursys.Domain.Interfaces.Services
{
    public interface IBuscaParametroConfiguracaoService
    {
        T GetParametroConfiguracao<T>(ParametroOrgCodigoEnum parametro, int orgId, string codigoInternoColaborador);
        T GetParametroConfiguracao<T>(ParametroOrgCodigoFrontEndEnum parametro, int orgId, string codigoInternoColaborador);
    }
}