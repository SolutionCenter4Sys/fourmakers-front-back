using DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoMenu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Usuario.GestaoDeAcesso
{
    public interface IRecursoMenuRepository
    {
        Task<IEnumerable<RecursoMenuResult>> ListarRecursoMenusAsync();
        Task<RecursoMenuResult> ObterRecursoMenuPorIdAsync(Guid id);
        Task<RecursoMenuResult> ObterRecursoMenuPorCodigoRecursoAsync(string codigoRecurso);
        Task<RecursoMenuResult> ObterRecursoMenuPorCodigoRecursoMenuAsync(string codigoRecursoMenu);
        Task<RecursoMenuResult> InserirRecursoMenuAsync(RecursoMenuInput parametroRepositoryInput);
        Task<RecursoMenuResult> AtualizarRecursoMenuAsync(RecursoMenuInput parametroRepositoryInput);
        Task<bool> DeletarRecursoMenuAsync(Guid id);

    }

}
