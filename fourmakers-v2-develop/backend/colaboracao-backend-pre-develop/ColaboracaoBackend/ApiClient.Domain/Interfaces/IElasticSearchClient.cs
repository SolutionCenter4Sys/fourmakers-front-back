using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces;

public interface IElasticSearchClient
{
    Task SyncColaborador(string codigoInternoColaborador, string token);
}