using Core.Domain.Colaborador;
using Core.Domain.Vaga;
using Colaboracao.Core;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Vaga.Enums;
using SRS.Domain.Interfaces.Service.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DomainModel;
using DataTransferObject.Domain.Vaga;

namespace SRS.Domain.Impl.Service.Strategy
{
    /// <summary>
    /// Factory para obter a estratégia correta baseada no status.
    /// Implementa o padrão Strategy com registro de estratégias.
    /// </summary>
    public class MudancaStatusCandidaturaStrategyFactory
    {
        private readonly List<IMudancaStatusCandidaturaStrategy> _strategies;
        private readonly List<IValidacaoMudancaStatusStrategy> _validacoes;

        public MudancaStatusCandidaturaStrategyFactory(
            IBuscaColaboradorRepository buscaColaboradorRepository,
            IVagaFourmakersRepository vagaFourmakersRepository,
            ICandidaturaRepository candidaturaRepository,
            ILogCore log,
            Func<CandidaturaRecrutamentoDTO, string, Task> enviarEmailParaGestoresExternos)
        {
            _strategies = new List<IMudancaStatusCandidaturaStrategy>();
            _validacoes = new List<IValidacaoMudancaStatusStrategy>();

            // Registrar todas as estratégias de execução
            _strategies.Add(new EntrevistaInicialStatusStrategy(vagaFourmakersRepository));
            _strategies.Add(new PesquisaInternaStatusStrategy(vagaFourmakersRepository));
            _strategies.Add(new AnaliseGestorStatusStrategy(vagaFourmakersRepository, log));
            _strategies.Add(new AnaliseGestorEnvioEmailStatusStrategy(log, enviarEmailParaGestoresExternos));
            _strategies.Add(new TestesComportamentaisStatusStrategy(vagaFourmakersRepository, log));
            _strategies.Add(new TestesTecnicosStatusStrategy(vagaFourmakersRepository, log));
            _strategies.Add(new EntrevistaComClienteStatusStrategy(vagaFourmakersRepository, log));
            _strategies.Add(new CartaOfertaAceitaStatusStrategy(vagaFourmakersRepository, log));
            _strategies.Add(new AprovadoStatusStrategy(vagaFourmakersRepository, log));
            _strategies.Add(new AprovadoQualificadoStrategy(buscaColaboradorRepository, log));
            _strategies.Add(new ReprovadoStatusStrategy(vagaFourmakersRepository, log));
            _strategies.Add(new ContratadoStatusStrategy(vagaFourmakersRepository, candidaturaRepository, log));
            _strategies.Add(new DeclinouStatusStrategy(vagaFourmakersRepository, log));
            _strategies.Add(new RemoverContratadoDaVagaStrategy(vagaFourmakersRepository, log));

            // Registrar todas as estratégias de validação
            _validacoes.Add(new ValidarLimiteContratadosStrategy());
        }

        /// <summary>
        /// Obtém todas as estratégias que devem executar para a transição de status
        /// </summary>
        public IEnumerable<IMudancaStatusCandidaturaStrategy> ObterEstrategias(int statusAnteriorId, int novoStatusId, VagaRecrutamentoDTO vaga)
        {
            return _strategies.Where(s => s.DeveExecutar(statusAnteriorId, novoStatusId, vaga));
        }

        /// <summary>
        /// Obtém todas as validações que devem executar para o novo status
        /// </summary>
        public IEnumerable<IValidacaoMudancaStatusStrategy> ObterValidacoes(int novoStatusId)
        {
            return _validacoes.Where(v => v.AplicaParaStatus(novoStatusId));
        }
    }
}

