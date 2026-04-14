using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.Domain.Interfaces.Services
{
    public interface IColaboradorSugestaoService
    {
        Task<List<string>> ListarEmpresasRelacionadas(string nomeEmpresa, int limite, int cursor, int orgId);
    }
}