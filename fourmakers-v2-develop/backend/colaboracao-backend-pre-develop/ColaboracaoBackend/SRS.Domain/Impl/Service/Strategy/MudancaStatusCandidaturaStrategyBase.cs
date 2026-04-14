using DataTransferObject.Domain.Vaga;
using SRS.Domain.Interfaces.Service.Strategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Impl.Service.Strategy
{
    /// <summary>
    /// Classe base abstrata para estratégias de mudança de status.
    /// Fornece implementação padrão vazia para métodos opcionais.
    /// </summary>
    public abstract class MudancaStatusCandidaturaStrategyBase : IMudancaStatusCandidaturaStrategy
    {
        public abstract bool DeveExecutar(int statusAnteriorId, int novoStatusId, VagaRecrutamentoDTO vaga);

        public virtual async Task<string> ExecutarAsync(MudancaStatusCandidaturaContext context)
        {
            // Implementação padrão vazia - pode ser sobrescrita pelas classes filhas
            await Task.CompletedTask;
            return string.Empty;
        }
    }
}

