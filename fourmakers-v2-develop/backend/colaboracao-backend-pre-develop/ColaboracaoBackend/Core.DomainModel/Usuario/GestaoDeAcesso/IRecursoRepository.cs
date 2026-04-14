using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Usuario.GestaoDeAcesso
{
    public interface IRecursoRepository
    {
        Task<IEnumerable<RecursoResult>> ListarRecursosAsync();
        Task<IEnumerable<RecursoMenuAninhadoResult>> ListarRecursosVisaoMenuAsync(string codigoInternoColaborador, int orgId);
        Task<RecursoResult> ObterRecursoPorCodigoRecursoAsync(string codigo);
        Task<RecursoResult> InserirRecursoAsync(RecursoInput parametroRepositoryInput);
        Task<RecursoResult> AtualizarRecursoAsync(RecursoInput parametroRepositoryInput);
        Task<bool> DeletarRecursoAsync(string codigoRecurso);
        Task<bool> DeletarTodosRecursosAsync();

    }

}
