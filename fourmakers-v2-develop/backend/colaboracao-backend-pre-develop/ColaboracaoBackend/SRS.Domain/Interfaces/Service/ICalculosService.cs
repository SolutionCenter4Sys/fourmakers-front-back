using DataTransferObject.Domain.Calculos;
using DataTransferObject.Domain.Match;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service
{
    public interface ICalculosService
    {
        Task<CalcularSalarioLiquidoOutputDTO> CalcularSalarioLiquidoAsync(CalcularSalarioLiquidoInputDTO input, InssTabelaDTO tabelaInss = null, IrrfTabelaDTO tabelaIrrf = null, IrrfReducaoDTO tabelaReducaoIrrf = null, bool calculoIterno = false);
        Task<SimularRemuneracaoTotalOutputDTO> SimularRemuneracaoTotalAsync(SimularRemuneracaoTotalInputDTO input, int orgId);
        Task<CandidatosMatchResponse> CalcularAderenciaColaboradorVaga(string vagaId, string codigoInternoColaborador);
    }
}
