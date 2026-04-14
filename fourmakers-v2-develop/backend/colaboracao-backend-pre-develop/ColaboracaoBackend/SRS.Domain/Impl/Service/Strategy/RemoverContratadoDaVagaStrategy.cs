using Colaboracao.Core;
using Colaboracao.Helper;
using Core.Domain.Vaga;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using SRS.Infra.Constantes;
using System.Threading.Tasks;

namespace SRS.Domain.Impl.Service.Strategy
{
    public class RemoverContratadoDaVagaStrategy : MudancaStatusCandidaturaStrategyBase
    {
        private readonly IVagaFourmakersRepository _vagaRepository;
        private readonly ILogCore _log;

        public RemoverContratadoDaVagaStrategy(IVagaFourmakersRepository vagaRepository, ILogCore log)
        {
            _vagaRepository = vagaRepository;
            _log = log;
        }

        public override bool DeveExecutar(int statusAnteriorId, int novoStatusId, VagaRecrutamentoDTO vaga)
        {
            return statusAnteriorId == StatusCandidaturaRecrutamento.Contratado.ToInt() 
                && novoStatusId != StatusCandidaturaRecrutamento.Contratado.ToInt();
        }

        public override async Task<string> ExecutarAsync(MudancaStatusCandidaturaContext context)
        {
            if (context.Vaga.CandidatosContratados != 0)
                context.Vaga.CandidatosContratados--;

            await _vagaRepository.AtualizarVaga(context.Vaga, UsuarioSistemicoConstants.SYSTEMIC_CODIGO_COLABORADOR);

            return string.Empty;
        }
    }
}

