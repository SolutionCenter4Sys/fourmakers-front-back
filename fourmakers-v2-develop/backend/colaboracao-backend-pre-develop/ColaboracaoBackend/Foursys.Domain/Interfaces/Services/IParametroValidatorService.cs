using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Fourmakers.Parametro;
using System.Threading.Tasks;

namespace Foursys.Domain.Interfaces.Services
{
    public interface IParametroValidatorService
    {
        Task ValidaParametro(ParametroRepositoryInput parametroRepositoryInput, CRUDEnum create);
    }
}