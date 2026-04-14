using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.DomainModel
{
    public interface IFirebaseRepository
    {
        Task<string> BuscarDeviceTokenApp(string codigoColaborador);
        Task<List<string>> BuscarDeviceTokensAppEmLote(List<string> codigosColaboradores);
        Task<string> BuscarEmailColaborador(string codigoInternoColaborador);
    }
}
