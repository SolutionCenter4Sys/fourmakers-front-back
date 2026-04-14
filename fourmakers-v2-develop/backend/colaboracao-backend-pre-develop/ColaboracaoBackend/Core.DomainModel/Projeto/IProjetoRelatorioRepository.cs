using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Projeto
{
    public interface IProjetoRelatorioRepository
    {
        Task<List<dynamic>> ListarProjetosParaExportacao(int orgId);
    }
}