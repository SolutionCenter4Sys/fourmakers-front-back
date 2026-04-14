using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario.GestaoDeAcesso.Recurso;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Usuario.Domain.Interfaces.Services.GestaoDeAcesso.Recurso
{
    public interface IRecursoService
    {
        Task<ApiGenericResult<IEnumerable<RecursoResult>>> ListarRecursos(string cpfRequest, int orgId);
        Task<ApiGenericResult<IEnumerable<RecursoMenuAninhadoResult>>> ListarRecursosVisaoMenu(string cpfRequest, int orgId);
        Task<ApiGenericResult<RecursoResult>> ObterRecursoPorCodigoRecurso(string codigoRecurso, string CpfRequest, int orgId);
        Task<ApiGenericResult<List<RecursoResult>>> InserirRecursos(List<RecursoInput> recursoInputs, bool forcarExclusao, string cpfRequest, int orgId);
        Task<ApiGenericResult<RecursoResult>> AtualizarRecurso(RecursoInput recursoInput, string codigoRecurso, string cpfRequest, int orgId);
        Task<ApiGenericResult> DeletarRecurso(string codigoRecurso, bool forcarExclusao, string cpfRequest, int orgId);
        Task<ApiGenericResult> DeletarTodosRecurso(string cpfRequest, int orgId);
        Task<ApiGenericResult<List<RecursoResult>>> ObterUltimaInsercaoRecursoMenuLog(string cpf, int orgId);
    }
}
