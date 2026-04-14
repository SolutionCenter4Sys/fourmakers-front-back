using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Vaga;
using SRS.Domain.Impl.Service.Strategy;
using System.Threading.Tasks;

namespace SRS.Domain.Interfaces.Service.Strategy
{
    /// <summary>
    /// Interface para estratégias de validação antes da mudança de status.
    /// </summary>
    public interface IValidacaoMudancaStatusStrategy
    {
        /// <summary>
        /// Verifica se esta validação se aplica ao status
        /// </summary>
        bool AplicaParaStatus(int statusId);

        /// <summary>
        /// Executa a validação antes da mudança de status
        /// </summary>
        /// <param name="context">Contexto com todas as informações necessárias</param>
        /// <returns>Mensagem de erro se a validação falhar, null se passar</returns>
        Task<string> ValidarAsync(MudancaStatusCandidaturaContext context);
    }
}

