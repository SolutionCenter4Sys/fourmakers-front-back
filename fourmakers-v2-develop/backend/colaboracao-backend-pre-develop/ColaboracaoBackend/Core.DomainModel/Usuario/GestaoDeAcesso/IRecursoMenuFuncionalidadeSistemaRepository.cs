using DataTransferObject.Domain.Usuario.GestaoDeAcesso.RecursoFuncionalidadeSistema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Usuario.GestaoDeAcesso
{
    public interface IRecursoMenuFuncionalidadeSistemaRepository
    {
        Task<RecursoFuncionalidadeSistemaResult> InserirRecursoMenuFuncionalidadeSistemaAsync(RecursoMenuFuncionalidadeSistemaInput parametroRepositoryInput);
        Task<IEnumerable<RecursoFuncionalidadeSistemaResult>> ObterRecursoMenuFuncionalidadeSistemaPorCodigoRecursoMenuAsync(string codigoRecurso);
        Task<RecursoFuncionalidadeSistemaResult> ObterRecursoMenuFuncionalidadeSistemaPorCodigoEFuncionalidadeSistemaIdAsync(string codigo, int tbFuncionalidadeSistemaId);
        Task<bool> DeletarRecursoFuncionalidadeSistemaAsync(RecursoMenuFuncionalidadeSistemaInput input);


    }

}
