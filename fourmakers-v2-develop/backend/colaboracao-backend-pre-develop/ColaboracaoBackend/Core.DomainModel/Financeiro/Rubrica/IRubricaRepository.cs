using DataTransferObject.Domain.Financeiro.Rubrica.Rubrica;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Domain.Financeiro.Rubrica
{
    public interface IRubricaRepository
    {
        Task<IEnumerable<RubricaResult>> ListarRubricasAsync(int orgid, bool somenteComTemplates = false);
        Task<RubricaResult> ObterRubricaPorIdAsync(Guid id);
        Task<RubricaResult> ObterRubricaPorCodigoAsync(string codigo, int orgId);
        Task<RubricaResult> ObterRubricaPorDescricaoAsync(string descricao, int orgId);
        Task<RubricaResult> InserirRubricaAsync(RubricaInput parametroRepositoryInput);
        Task<RubricaResult> AtualizarRubricaAsync(RubricaInput parametroRepositoryInput);
        Task<bool> DeletarRubricaAsync(Guid id);
        
    }

}
