using Colaboracao.Core;
using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Core.Domain.Vaga;
using Core.DomainModel;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using DocumentFormat.OpenXml.InkML;
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
    public class ReprovadoStatusStrategy : MudancaStatusCandidaturaStrategyBase
    {
        private readonly IVagaFourmakersRepository _vagaRepository;
        private readonly ILogCore _log;
        private const int QUANTIDADE_CANDIDATOS_MINIMO_PARA_ENVIAR_VAGA_PARA_ENTREVISTA_COM_CLIENTE = 3;

        public ReprovadoStatusStrategy(
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

            if (novoStatusId != StatusCandidaturaRecrutamento.Reprovado.ToInt())
                return false;

            return true;
        }

        public override async Task<string> ExecutarAsync(MudancaStatusCandidaturaContext context)
        {
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

            string mensagemRetorno = string.Empty;

            if (quantidadeEmAndamentoComCliente < (QUANTIDADE_CANDIDATOS_MINIMO_PARA_ENVIAR_VAGA_PARA_ENTREVISTA_COM_CLIENTE * context.Vaga.PosicoesRestantes))
            {
                if (!(context.Vaga.StatusVagaCod == StatusVagaRecrutamento.ProcurandoCandidatos.ToInt().ToString()))
                {
                    context.Vaga.StatusVagaCod = StatusVagaRecrutamento.ProcurandoCandidatos.ToInt().ToString();
                    mensagemRetorno = $"Vaga sob o codigo {context.Vaga.Codigo} movida para Procurando Candidatos";
                    await _vagaRepository.AtualizarVaga(context.Vaga, UsuarioSistemicoConstants.SYSTEMIC_CODIGO_COLABORADOR);
                }
            }
            else
            {
                if (!(context.Vaga.StatusVagaCod == StatusVagaRecrutamento.EntrevistaComGestorDaVaga.ToInt().ToString()))
                {
                    context.Vaga.StatusVagaCod = StatusVagaRecrutamento.EntrevistaComGestorDaVaga.ToInt().ToString();
                    mensagemRetorno = $"Vaga sob o codigo {context.Vaga.Codigo} movida para Entrevista Com Cliente";
                    await _vagaRepository.AtualizarVaga(context.Vaga, UsuarioSistemicoConstants.SYSTEMIC_CODIGO_COLABORADOR);
                }
            }

            return mensagemRetorno;
        }
    }
}

