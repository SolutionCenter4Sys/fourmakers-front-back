using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Vaga;
using SRS.Domain.Impl.Service.Strategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service.Strategy
{
    /// <summary>
    /// Interface para estratégias de mudança de status de candidatura.
    /// Cada status pode ter regras de negócio específicas implementadas através desta interface.
    /// </summary>
    public interface IMudancaStatusCandidaturaStrategy
    {
        /// <summary>
        /// Verifica se esta estratégia deve executar para a transição de status
        /// </summary>
        /// <param name="statusAnteriorId">ID do status anterior</param>
        /// <param name="novoStatusId">ID do novo status</param>
        /// <returns>True se a estratégia deve executar</returns>
        bool DeveExecutar(int statusAnteriorId, int novoStatusId, VagaRecrutamentoDTO vaga);

        /// <summary>
        /// Executa as regras de negócio específicas para a mudança de status
        /// </summary>
        /// <param name="context">Contexto com todas as informações necessárias para a mudança de status</param>
        /// <returns>Mensagem de retorno opcional para o usuário</returns>
        Task<string> ExecutarAsync(MudancaStatusCandidaturaContext context);
    }
}

