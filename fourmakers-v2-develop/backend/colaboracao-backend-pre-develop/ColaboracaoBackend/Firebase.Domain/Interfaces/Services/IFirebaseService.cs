using System.Collections.Generic;
using System.Threading.Tasks;

namespace Firebase.Domain.Interfaces.Services
{
    public interface IFirebaseService
    {
        Task<bool> EnviaPush(string token, string titulo, string mensagem);
        Task<bool> EnviaPushEmLote(List<string> tokens, string titulo, string mensagem);
        Task<bool> EnviarNotificacaoPushApp(string codigoColaborador, string titulo, string mensagem);
        Task<bool> EnviarNotificacaoPushAppEmLote(List<string> codigosColaboradores, string titulo, string mensagem);
    }
}