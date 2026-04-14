using Colaboracao.Core;
using Colaboracao.Helper;
using Core.Domain.Colaborador;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using System.Threading.Tasks;

namespace SRS.Domain.Impl.Service.Strategy
{
    /// <summary>
    /// Estratégia para o status Aprovado.
    /// Regra: Qualifica o colaborador quando aprovado.
    /// </summary>
    public class AprovadoQualificadoStrategy : MudancaStatusCandidaturaStrategyBase
    {
        private readonly IBuscaColaboradorRepository _buscaColaboradorRepository;
        private readonly ILogCore _log;

        public AprovadoQualificadoStrategy(IBuscaColaboradorRepository buscaColaboradorRepository, ILogCore log)
        {
            _buscaColaboradorRepository = buscaColaboradorRepository;
            _log = log;
        }

        public override bool DeveExecutar(int statusAnteriorId, int novoStatusId, VagaRecrutamentoDTO vaga)
        {
            return novoStatusId == StatusCandidaturaRecrutamento.Aprovado.ToInt();
        }

        public override async Task<string> ExecutarAsync(MudancaStatusCandidaturaContext context)
        {
            await _buscaColaboradorRepository.QualificarColaborador(
                context.Candidatura.CodColaborador, 
                context.CodColaborador);

            return "Candidato qualificado";
        }
    }
}

