using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Fourmakers.ParametroConfiguracao;
using System.Threading.Tasks;

namespace Foursys.Domain.Interfaces.Services
{
    public interface IParametroConfiguracaoValidatorService
    {
        Task ValidaParametroConfiguracao(ParametroConfiguracaoRepositoryInput parametroConfiguracaoRepositoryInput, CRUDEnum create);
    }
}