using DataTransferObject.Domain.Cep;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface ICepClient
    {
        Task<ConsultaCepDTO> ConsultaCep(string cep);
    }
}