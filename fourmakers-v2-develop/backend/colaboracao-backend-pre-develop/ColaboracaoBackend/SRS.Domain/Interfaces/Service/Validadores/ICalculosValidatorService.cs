using DataTransferObject.Domain.Calculos;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service.Validadores
{
    public interface ICalculosValidatorService
    {
        Task ValidarCalcularSalarioLiquido(CalcularSalarioLiquidoInputDTO input);
        Task ValidarSimularRemuneracaoTotal(SimularRemuneracaoTotalInputDTO input);
    }
}
