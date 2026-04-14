using System.Threading.Tasks;

namespace Core.DomainModel
{
    public interface ITokenFileRepository
    {
        Task<bool> ValidarTokenArquivo(string token, string nomeArquivo);
        Task InserirTokenArquivo(string token, string nomeArquivo);
        Task DeletarTokenArquivo(string nomeArquivo);
        Task AtualizarNomeArquivo(string nomeAtual, string nomeNovo);
    }
}