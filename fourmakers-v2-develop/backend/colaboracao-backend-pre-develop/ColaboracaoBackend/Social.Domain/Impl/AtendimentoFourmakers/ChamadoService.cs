using Core.Domain.Social.AtendimentoFourmakers;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using Logs.Infra.Attributes;
using Social.Domain.Interfaces.AtendimentoFourmakers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Social.Domain.Impl.AtendimentoFourmakers
{
    [LogDomainClass]
    public class ChamadoService : IChamadoService
    {
        private readonly IChamadoRepository _chamadoRepository;
        private readonly IAuditoriaAtendimentoRepository _auditoriaRepository;

        private static readonly HashSet<string> StatusValidos = new(StringComparer.OrdinalIgnoreCase)
            { "aberto", "em_andamento", "aguardando", "resolvido", "cancelado" };

        private static readonly HashSet<string> PrioridadeValidas = new(StringComparer.OrdinalIgnoreCase)
            { "baixa", "media", "alta", "urgente" };

        public ChamadoService(IChamadoRepository chamadoRepository, IAuditoriaAtendimentoRepository auditoriaRepository)
        {
            _chamadoRepository = chamadoRepository;
            _auditoriaRepository = auditoriaRepository;
        }

        public async Task<ApiGenericResult<ChamadoResult>> CriarAsync(CriarChamadoInput input, string codigoInternoColaborador, string nomeColaborador, int orgId)
        {
            var result = new ApiGenericResult<ChamadoResult>();

            if (string.IsNullOrWhiteSpace(input?.Assunto))
                throw new ArgumentException("Assunto é obrigatório.");

            var assunto = Truncar(input.Assunto.Trim(), 500);
            var descricao = Truncar(input.Descricao?.Trim(), 8000);
            var cooperadoLabel = string.IsNullOrEmpty(nomeColaborador) ? "Colaborador" : nomeColaborador;

            var insertInput = new ChamadoInsertInput
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                CooperadoLabel = cooperadoLabel,
                Assunto = assunto,
                Descricao = descricao,
                Status = "aberto",
                Prioridade = "media"
            };

            var id = await _chamadoRepository.InserirAsync(insertInput, orgId);
            result.Retorno = await _chamadoRepository.ObterPorIdAsync(id, orgId);

            return result;
        }

        public async Task<ApiGenericResult<ChamadoResult>> AtualizarAsync(string id, AtualizarChamadoInput input, string codigoInternoColaborador, string nomeColaborador, int orgId)
        {
            var result = new ApiGenericResult<ChamadoResult>();

            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id do chamado é obrigatório.");

            if (input == null || (input.Status == null && input.Prioridade == null && input.NotasResolucao == null && input.RespostaPublica == null))
                throw new ArgumentException("Pelo menos um campo deve ser fornecido.");

            if (input.Status != null && !StatusValidos.Contains(input.Status))
                throw new ArgumentException($"Status inválido: {input.Status}");

            if (input.Prioridade != null && !PrioridadeValidas.Contains(input.Prioridade))
                throw new ArgumentException($"Prioridade inválida: {input.Prioridade}");

            var existente = await _chamadoRepository.ObterPorIdAsync(id, orgId);
            if (existente == null)
                throw new ApplicationException("Chamado não encontrado.");

            var updateInput = new ChamadoUpdateInput
            {
                Status = input.Status,
                Prioridade = input.Prioridade,
                NotasResolucao = Truncar(input.NotasResolucao, 8000),
                RespostaPublica = Truncar(input.RespostaPublica, 4000)
            };

            await _chamadoRepository.AtualizarAsync(id, updateInput, orgId);

            var camposAlterados = new List<string>();
            if (input.Status != null) camposAlterados.Add($"status → {input.Status}");
            if (input.Prioridade != null) camposAlterados.Add($"prioridade → {input.Prioridade}");
            if (input.NotasResolucao != null) camposAlterados.Add("notas de resolução alteradas");
            if (input.RespostaPublica != null) camposAlterados.Add("resposta pública alterada");

            await _auditoriaRepository.InserirAsync(new AuditoriaInsertInput
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                ActorLabel = nomeColaborador ?? "Sistema",
                Acao = "ticket.updated",
                Detalhe = string.Join("; ", camposAlterados),
                Payload = JsonSerializer.Serialize(input),
                TipoEntidade = "chamado",
                EntidadeId = id
            }, orgId);

            result.Retorno = await _chamadoRepository.ObterPorIdAsync(id, orgId);
            return result;
        }

        public async Task<ApiGenericResult<List<ChamadoResult>>> ListarAsync(string codigoInternoColaborador, int orgId, string status = null)
        {
            var result = new ApiGenericResult<List<ChamadoResult>>();
            var chamados = await _chamadoRepository.ListarAsync(orgId, codigoInternoColaborador, status);
            result.Retorno = chamados.ToList();
            return result;
        }

        public async Task<ApiGenericResult<ChamadoResult>> ObterPorIdAsync(string id, string codigoInternoColaborador, int orgId)
        {
            var result = new ApiGenericResult<ChamadoResult>();
            var chamado = await _chamadoRepository.ObterPorIdAsync(id, orgId);

            if (chamado == null)
                throw new ApplicationException("Chamado não encontrado.");

            if (!string.IsNullOrEmpty(codigoInternoColaborador) && chamado.CodigoInternoColaborador != codigoInternoColaborador)
                throw new ApplicationException("Chamado não encontrado.");

            result.Retorno = chamado;
            return result;
        }

        public async Task<ApiGenericResult<DistribuicaoStatusResult>> ObterDistribuicaoStatusAsync(int orgId)
        {
            var result = new ApiGenericResult<DistribuicaoStatusResult>();
            var itens = (await _chamadoRepository.ObterDistribuicaoStatusAsync(orgId)).ToList();
            result.Retorno = new DistribuicaoStatusResult
            {
                Itens = itens,
                Total = itens.Sum(i => i.Valor)
            };
            return result;
        }

        private static string Truncar(string texto, int maxChars)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            return texto.Length <= maxChars ? texto : texto[..maxChars];
        }
    }
}
