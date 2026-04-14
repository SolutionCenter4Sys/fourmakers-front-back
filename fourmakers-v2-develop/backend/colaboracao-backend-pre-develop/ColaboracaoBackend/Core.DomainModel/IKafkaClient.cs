using System.Threading.Tasks;

namespace Core.Domain
{
    public interface IKafkaClient
    {
        Task ProcessaItem(string chave, string valor, int intervaloTentativas, int tentativas);
    }
}