using DataTransferObject.Domain.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiClient.Domain.Interfaces
{
    public interface IFirebaseClient
    {
        Task<StatusResult> EnviaPush(string token, string titulo, string mensagem);
        Task<StatusResult> EnviaPushEmLote(List<string> tokens, string titulo, string mensagem);
    }
}