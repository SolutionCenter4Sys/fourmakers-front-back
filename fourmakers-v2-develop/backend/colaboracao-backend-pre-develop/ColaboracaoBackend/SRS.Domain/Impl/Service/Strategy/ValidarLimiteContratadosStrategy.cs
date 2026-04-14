using Colaboracao.Helper;
using DataTransferObject.Domain.Vaga.Enums;
using SRS.Domain.Interfaces.Service.Strategy;
using System.Threading.Tasks;

namespace SRS.Domain.Impl.Service.Strategy
{
    /// <summary>
    /// Estratégia de validação para verificar limite de candidatos contratados.
    /// </summary>
    public class ValidarLimiteContratadosStrategy : IValidacaoMudancaStatusStrategy
    {
        public bool AplicaParaStatus(int statusId)
        {
            return statusId == StatusCandidaturaRecrutamento.Contratado.ToInt();
        }

        public async Task<string> ValidarAsync(MudancaStatusCandidaturaContext context)
        {
            await Task.CompletedTask;
            
            var vaga = context.Vaga;
            if ((vaga.NumeroDeVagas - vaga.CandidatosContratados) <= 0)
            {
                return "O limite de candidatos contratados para esta vaga já foi atingido.";
            }

            return null;
        }
    }
}

