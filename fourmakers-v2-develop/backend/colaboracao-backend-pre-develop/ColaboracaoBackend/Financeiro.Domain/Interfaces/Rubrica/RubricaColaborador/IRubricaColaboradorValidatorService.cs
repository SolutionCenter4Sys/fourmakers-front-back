using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using System.Threading.Tasks;

namespace Financeiro.Domain.Interfaces.Rubrica.RubricaColaborador
{
    public interface IRubricaColaboradorValidatorService
    {
        Task ValidaRubricaColaborador(RubricaColaboradorInput rubricaColaboradorInput, CRUDEnum create);
    }
}
