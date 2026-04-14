using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using System.Threading.Tasks;

namespace Financeiro.Domain.Interfaces.Rubrica.Rubrica
{
    public interface IRubricaValidatorService
    {
        Task ValidaRubrica(RubricaInput rubricaInput, CRUDEnum create);
    }
}
