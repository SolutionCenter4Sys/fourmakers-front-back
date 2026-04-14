using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firebase.Domain.Interfaces
{
    public interface IFirebaseSDK
    {
        Task EnviaPushEmLote(List<string> tokens, string titulo, string mensagem);
        Task EnviaPush(string token, string titulo, string mensagem);
    }
}