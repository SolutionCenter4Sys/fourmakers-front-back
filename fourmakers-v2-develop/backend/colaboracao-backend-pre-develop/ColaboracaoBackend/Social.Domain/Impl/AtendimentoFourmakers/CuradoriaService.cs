using Core.Domain.Social.AtendimentoFourmakers;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using Logs.Infra.Attributes;
using Social.Domain.Interfaces.AtendimentoFourmakers;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Social.Domain.Impl.AtendimentoFourmakers
{
    [LogDomainClass]
    public class CuradoriaService : ICuradoriaService
    {
        private readonly IAssistenteChatLogRepository _chatLogRepository;
        private readonly IAuditoriaAtendimentoRepository _auditoriaRepository;

        private static readonly HashSet<string> StatusValidos = new(StringComparer.OrdinalIgnoreCase)
            { "open", "in_review", "resolved", "ignored" };

        public CuradoriaService(IAssistenteChatLogRepository chatLogRepository, IAuditoriaAtendimentoRepository auditoriaRepository)
        {
            _chatLogRepository = chatLogRepository;
            _auditoriaRepository = auditoriaRepository;
        }

        public async Task<ApiGenericResult<CuradoriaLogResult>> AtualizarLogAsync(string id, AtualizarCuradoriaInput input, string codigoInternoColaborador, string nomeColaborador, int orgId)
        {
            var result = new ApiGenericResult<CuradoriaLogResult>();

            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id do log é obrigatório.");

            if (input?.StatusCuradoria != null && !StatusValidos.Contains(input.StatusCuradoria))
                throw new ArgumentException($"StatusCuradoria inválido: {input.StatusCuradoria}");

            var notaCurador = input?.NotaCurador;
            if (notaCurador != null && notaCurador.Length > 4000)
                notaCurador = notaCurador[..4000];

            var log = await _chatLogRepository.ObterPorIdAsync(id, orgId);
            if (log == null)
                throw new ApplicationException("Log não encontrado.");

            var atualizado = await _chatLogRepository.AtualizarCuradoriaAsync(
                id, input?.StatusCuradoria, notaCurador, orgId);

            if (!atualizado)
                throw new ApplicationException("Falha ao atualizar curadoria.");

            var detalhes = new List<string>();
            if (input?.StatusCuradoria != null) detalhes.Add($"status → {input.StatusCuradoria}");
            if (input?.NotaCurador != null) detalhes.Add("nota do curador alterada");

            await _auditoriaRepository.InserirAsync(new AuditoriaInsertInput
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                ActorLabel = nomeColaborador ?? "Sistema",
                Acao = "curation.log_updated",
                Detalhe = string.Join("; ", detalhes),
                Payload = JsonSerializer.Serialize(input),
                TipoEntidade = "chat_log",
                EntidadeId = id
            }, orgId);

            result.Retorno = new CuradoriaLogResult
            {
                Id = id,
                StatusCuradoria = input?.StatusCuradoria ?? log.StatusCuradoria,
                NotaCurador = notaCurador ?? log.NotaCurador
            };

            return result;
        }
    }
}
