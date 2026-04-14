using System.Collections.Generic;

namespace Usuario.Domain.Interfaces.Services
{
    public interface IUsuarioCacheService
    {
        IEnumerable<RecursoMenuAninhadoResult> ObterListarRecursosVisaoMenuCache(string codigoInternoColaborador, int orgId);
        void AtualizarRecursosVisaoMenuCache(IEnumerable<RecursoMenuAninhadoResult> recursoMenuAninhado, string codigoInternoColaborador, int orgId);

    }
}