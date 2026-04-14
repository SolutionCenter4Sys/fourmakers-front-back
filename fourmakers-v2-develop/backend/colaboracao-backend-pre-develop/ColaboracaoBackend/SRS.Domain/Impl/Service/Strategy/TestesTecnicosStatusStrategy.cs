using Colaboracao.Core;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Vaga;
using Core.DomainModel;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using SRS.Infra.Constantes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SRS.Domain.Impl.Service.Strategy
{
    /// <summary>
    /// Estratégia para o status AnaliseGestor.
    /// Regras:
    /// 1. Verifica se há quantidade mínima de candidatos para enviar vaga para EntrevistaComCliente
    /// 2. Envia email para gestores externos em background
    /// </summary>
    public class TestesTecnicosStatusStrategy : MudancaStatusCandidaturaStrategyBase
    {
        private readonly IVagaFourmakersRepository _vagaRepository;
        private readonly ILogCore _log;
        private const int QUANTIDADE_CANDIDATOS_MINIMO_PARA_ENVIAR_VAGA_PARA_ENTREVISTA_COM_CLIENTE = 3;

        public TestesTecnicosStatusStrategy(
            IVagaFourmakersRepository vagaRepository,
            ILogCore log)
        {
            _vagaRepository = vagaRepository;
            _log = log;
        }

        public override bool DeveExecutar(int statusAnteriorId, int novoStatusId, VagaRecrutamentoDTO vaga)
        {
            if (vaga.StatusVagaCod == StatusVagaRecrutamento.EmFoco.ToInt().ToString())
                return false;

            if (novoStatusId != StatusCandidaturaRecrutamento.TestesTecnicos.ToInt())
                return false;

            var quantidadeEmAnaliseGestor = vaga.QuantidadeCandidatosPorEstagio?
                .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.AnaliseDoCvPeloGestor.ToInt())
                ?.FirstOrDefault()?.Quantidade ?? 0;
            var quantidadeEmTestesComportamentais = vaga.QuantidadeCandidatosPorEstagio?
                .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.TestesComportamentais.ToInt())
                ?.FirstOrDefault()?.Quantidade ?? 0;
            var quantidadeEmTestesTecnicos = vaga.QuantidadeCandidatosPorEstagio?
                .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.TestesTecnicos.ToInt())
                ?.FirstOrDefault()?.Quantidade ?? 0;
            var quantidadeEmEntrevistaComCliente = vaga.QuantidadeCandidatosPorEstagio?
                .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.EntrevistaComGestorDaVaga.ToInt())
                ?.FirstOrDefault()?.Quantidade ?? 0;
            var quantidadeEmCartaOferta = vaga.QuantidadeCandidatosPorEstagio?
                .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.CartaOfertaAceita.ToInt())
                ?.FirstOrDefault()?.Quantidade ?? 0;
            var quantidadeEmAprovado = vaga.QuantidadeCandidatosPorEstagio?
                .Where(m => m.IdStatus == StatusCandidaturaRecrutamento.Aprovado.ToInt())
                ?.FirstOrDefault()?.Quantidade ?? 0;

            var quantidadeEmAndamentoComCliente = quantidadeEmAnaliseGestor
                + quantidadeEmTestesComportamentais
                + quantidadeEmTestesTecnicos
                + quantidadeEmEntrevistaComCliente
                + quantidadeEmCartaOferta
            + quantidadeEmAprovado;

            if (quantidadeEmAndamentoComCliente < (QUANTIDADE_CANDIDATOS_MINIMO_PARA_ENVIAR_VAGA_PARA_ENTREVISTA_COM_CLIENTE * vaga.PosicoesRestantes))
                return false;

            if (vaga.StatusVagaCod == StatusVagaRecrutamento.EntrevistaComGestorDaVaga.ToInt().ToString())
                return false;

            return true;
        }

        public override async Task<string> ExecutarAsync(MudancaStatusCandidaturaContext context)
        {
            context.Vaga.StatusVagaCod = StatusVagaRecrutamento.EntrevistaComGestorDaVaga.ToInt().ToString();
            await _vagaRepository.AtualizarVaga(context.Vaga, UsuarioSistemicoConstants.SYSTEMIC_CODIGO_COLABORADOR);

            return $"Vaga sob o codigo {context.Vaga.Codigo} movida para Entrevista Com Cliente";
        }
    }
}

