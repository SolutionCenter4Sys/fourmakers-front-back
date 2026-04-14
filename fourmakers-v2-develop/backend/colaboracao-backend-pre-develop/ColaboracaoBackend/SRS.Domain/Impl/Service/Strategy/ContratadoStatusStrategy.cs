using Colaboracao.Core;
using Colaboracao.Helper;
using Core.Domain.Vaga;
using Core.DomainModel;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using SRS.Infra.Constantes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SRS.Domain.Impl.Service.Strategy
{
    /// <summary>
    /// Estratégia para o status Contratado.
    /// Regras:
    /// 1. Incrementa candidatos contratados
    /// 2. Muda status da vaga para Contratação
    /// 3. Se houver posições restantes, cria nova vaga e copia candidatos
    /// </summary>
    public class ContratadoStatusStrategy : MudancaStatusCandidaturaStrategyBase
    {
        private readonly IVagaFourmakersRepository _vagaFourmakersRepository;
        private readonly ICandidaturaRepository _candidaturaRepository;
        private readonly ILogCore _log;
        private const int QUANTIDADE_CANDIDATOS_MINIMO_PARA_ENVIAR_VAGA_PARA_ENTREVISTA_COM_CLIENTE = 3;

        public ContratadoStatusStrategy(
            IVagaFourmakersRepository vagaFourmakersRepository,
            ICandidaturaRepository candidaturaRepository,
            ILogCore log)
        {
            _vagaFourmakersRepository = vagaFourmakersRepository;
            _candidaturaRepository = candidaturaRepository;
            _log = log;
        }

        public override bool DeveExecutar(int statusAnteriorId, int novoStatusId, VagaRecrutamentoDTO vaga)
        {
            if (vaga.StatusVagaCod == StatusVagaRecrutamento.EmFoco.ToInt().ToString())
                return false;

            return novoStatusId == StatusCandidaturaRecrutamento.Contratado.ToInt();
        }

        public override async Task<string> ExecutarAsync(MudancaStatusCandidaturaContext context)
        {
            var numeroVagasIniciais = context.Vaga.NumeroDeVagas;
            context.Vaga.CandidatosContratados++;
            context.Vaga.StatusVagaCod = StatusVagaRecrutamento.Contratacao.ToInt().ToString();
            context.Vaga.NumeroDeVagas = 1;
            await _vagaFourmakersRepository.AtualizarVaga(context.Vaga, UsuarioSistemicoConstants.SYSTEMIC_CODIGO_COLABORADOR);

            string mensagemRetorno = string.Empty;

            if (context.Vaga.PosicoesRestantes > 1)
            {
                var codigoVagaAtual = context.Vaga.Codigo;
                context.Vaga.IdVagaParent = context.Vaga.Id;
                context.Vaga.Id = Guid.NewGuid().ToString();
                context.Vaga.NumeroDeVagas = numeroVagasIniciais - 1;

                var quantidadeEmAnaliseGestor = context.Vaga.QuantidadeCandidatosPorEstagio?
                .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.AnaliseDoCvPeloGestor.ToInt())
                ?.FirstOrDefault()?.Quantidade ?? 0;
                var quantidadeEmTestesComportamentais = context.Vaga.QuantidadeCandidatosPorEstagio?
                    .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.TestesComportamentais.ToInt())
                    ?.FirstOrDefault()?.Quantidade ?? 0;
                var quantidadeEmTestesTecnicos = context.Vaga.QuantidadeCandidatosPorEstagio?
                    .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.TestesTecnicos.ToInt())
                    ?.FirstOrDefault()?.Quantidade ?? 0;
                var quantidadeEmEntrevistaComCliente = context.Vaga.QuantidadeCandidatosPorEstagio?
                    .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.EntrevistaComGestorDaVaga.ToInt())
                    ?.FirstOrDefault()?.Quantidade ?? 0;
                var quantidadeEmCartaOferta = context.Vaga.QuantidadeCandidatosPorEstagio?
                    .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.CartaOfertaAceita.ToInt())
                    ?.FirstOrDefault()?.Quantidade ?? 0;
                var quantidadeEmAprovado = context.Vaga.QuantidadeCandidatosPorEstagio?
                    .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.Aprovado.ToInt())
                    ?.FirstOrDefault()?.Quantidade ?? 0;

                var quantidadeEmAndamentoComCliente = quantidadeEmAnaliseGestor
                    + quantidadeEmTestesComportamentais
                    + quantidadeEmTestesTecnicos
                    + quantidadeEmEntrevistaComCliente
                    + quantidadeEmCartaOferta
                + quantidadeEmAprovado;

                if (quantidadeEmAndamentoComCliente < (QUANTIDADE_CANDIDATOS_MINIMO_PARA_ENVIAR_VAGA_PARA_ENTREVISTA_COM_CLIENTE * context.Vaga.NumeroDeVagas))
                {
                    context.Vaga.StatusVagaCod = StatusVagaRecrutamento.ProcurandoCandidatos.ToInt().ToString();
                    await _vagaFourmakersRepository.InserirVaga(context.Vaga, context.CodColaborador);
                    mensagemRetorno = $"A vaga sob o código {codigoVagaAtual} foi enviada para a Contratação. Foi criada a vaga sob o código {context.Vaga.Codigo} em Procurando Candidatos, pois a vaga {codigoVagaAtual} ainda tem {context.Vaga.PosicoesRestantes - 1} posições restantes.";
                }
                else
                {
                    context.Vaga.StatusVagaCod = StatusVagaRecrutamento.EntrevistaComGestorDaVaga.ToInt().ToString();
                    await _vagaFourmakersRepository.InserirVaga(context.Vaga, context.CodColaborador);
                    mensagemRetorno = $"A vaga sob o código {codigoVagaAtual} foi enviada para a Contratação. Foi criada a vaga sob o código {context.Vaga.Codigo} em Entrevista Com Cliente, pois a vaga {codigoVagaAtual} ainda tem {context.Vaga.PosicoesRestantes - 1} posições restantes.";
                }

                await _vagaFourmakersRepository.CopiarCandidatosEInformacoesComplementares(
                    context.Vaga.IdVagaParent, 
                    context.Vaga.Id, 
                    context.CodColaborador,
                    movidaAutomaticamente: true);
                await _candidaturaRepository.ExcluirCandidatura(
                    context.Candidatura.CodColaborador, 
                    context.Vaga.Id, 
                    context.CodColaborador);
            }

            return mensagemRetorno;
        }
    }
}

