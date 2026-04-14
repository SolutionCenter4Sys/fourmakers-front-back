using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Colaborador
{
    public interface IColaboradorSugestaoRepository
    {
        Task<List<string>> ListarEmpresasRelacionadas(string nomeEmpresa, int limite, int cursor, int orgId);
        Task<string> GetEmpresaPorNome(string nomeEmpresa, int orgId);
        Task<bool> AddEmpresaSugestao(string nome, string codigoInternoColaboradorCriacao, int orgId);
    }
}