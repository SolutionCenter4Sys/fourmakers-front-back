using DataTransferObject.Domain.Foursys;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IFoursysClient
    {
        Task<ListaUnidadesResult> ListarUnidades(string tokenUsuario);
        Task<ListaUnidadesResult> ListarUnidadesPorOrg(string tokenUsuario, int orgId);
    }
}