using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.SincronizarCRM;
using System.Threading.Tasks;

namespace CargaCore.Domain.Interfaces
{
    public interface ISincronizaCRMService
    {
        Task SincronizarCRM();
        Task<SincronizarCRMResult> GetDataUltimaSincronizacaoClientesEGestoresCRM();
    }
}