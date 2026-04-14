using System.Threading.Tasks;

namespace Comunicacao.Infra
{
    public interface IEvolutionApiService
    {
        Task<bool> EnviarMensagemTextoAsync(string numero, string texto);
        bool ValidarWebhookSecret(string secretRecebido);
    }
}
