using Colaboracao.Helper.Enum;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Usuario.Domain.Interfaces.Services.GestaoDeAcesso.Recurso
{
    public interface IRecursoValidatorService
    {
        Task ValidaRecurso(RecursoInput recursoInput, CRUDEnum create, List<RecursoInput> listaInput = default, bool forcarExclusao = false);
    }
}
