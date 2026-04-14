using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Financeiro.Domain.Interfaces.Rubrica.Rubrica
{
    public interface IRubricaService
    {
        Task<ApiGenericResult<IEnumerable<RubricaResult>>> ListarRubricas(string cpfRequest, int orgId, bool somenteComTemplates = false);
        Task<ApiGenericResult<RubricaResult>> ObterRubricaPorId(Guid id, string CpfRequest, int orgId, bool validaAcesso = true);
        Task<ApiGenericResult<RubricaResult>> InserirRubrica(RubricaInput rubricaInput, string cpfRequest, int orgId);
        Task<ApiGenericResult<RubricaResult>> AtualizarRubrica(RubricaInput rubricaInput, Guid id, string cpfRequest, int orgId);
        Task<ApiGenericResult> DeletarRubrica(Guid id, string cpfRequest, int orgId);
    }
}
