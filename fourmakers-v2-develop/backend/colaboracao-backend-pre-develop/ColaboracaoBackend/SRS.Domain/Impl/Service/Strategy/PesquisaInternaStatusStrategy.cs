using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Colaboracao.Helper;
using Core.Domain.Vaga;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.Vaga.Enums;
using DocumentFormat.OpenXml.InkML;
using SRS.Infra.Constantes;

namespace SRS.Domain.Impl.Service.Strategy
{
    public class PesquisaInternaStatusStrategy : MudancaStatusCandidaturaStrategyBase
    {
        private readonly IVagaFourmakersRepository _vagaRepository;

        public PesquisaInternaStatusStrategy(IVagaFourmakersRepository vagaRepository)
        {
            _vagaRepository = vagaRepository;
        }

        private const int QUANTIDADE_CANDIDATOS_MINIMO_PARA_ENVIAR_VAGA_PARA_ENTREVISTA_COM_CLIENTE = 3;//todo centralizar

        public override bool DeveExecutar(int statusAnteriorId, int novoStatusId, VagaRecrutamentoDTO vaga)
        {
            if (vaga.StatusVagaCod == StatusVagaRecrutamento.EmFoco.ToInt().ToString())
                return false;

            if (novoStatusId != StatusCandidaturaRecrutamento.PesquisaInterna.ToInt())
                return false;

            if (!(statusAnteriorId == StatusCandidaturaRecrutamento.AnaliseDoCvPeloGestor.ToInt()
                || statusAnteriorId == StatusCandidaturaRecrutamento.TestesComportamentais.ToInt()
                || statusAnteriorId == StatusCandidaturaRecrutamento.TestesTecnicos.ToInt()
                || statusAnteriorId == StatusCandidaturaRecrutamento.EntrevistaComGestorDaVaga.ToInt()
                || statusAnteriorId == StatusCandidaturaRecrutamento.CartaOfertaAceita.ToInt()
                || statusAnteriorId == StatusCandidaturaRecrutamento.Aprovado.ToInt()
                ))
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

            if (quantidadeEmAndamentoComCliente >= (QUANTIDADE_CANDIDATOS_MINIMO_PARA_ENVIAR_VAGA_PARA_ENTREVISTA_COM_CLIENTE * vaga.PosicoesRestantes))
                return false;

            return true;
        }

        public override async Task<string> ExecutarAsync(MudancaStatusCandidaturaContext context)
        {
            context.Vaga.StatusVagaCod = StatusVagaRecrutamento.ProcurandoCandidatos.ToInt().ToString();
            await _vagaRepository.AtualizarVaga(context.Vaga, UsuarioSistemicoConstants.SYSTEMIC_CODIGO_COLABORADOR);

            return $"Vaga sob o codigo {context.Vaga.Codigo}, foi movida para Procurando Candidatos.";
        }
    }
}
