using Colaboracao.Helper;
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
    public class AssistenteConfigService : IAssistenteConfigService
    {
        private readonly IAssistenteConfigRepository _configRepository;
        private readonly IAuditoriaAtendimentoRepository _auditoriaRepository;

        public AssistenteConfigService(IAssistenteConfigRepository configRepository, IAuditoriaAtendimentoRepository auditoriaRepository)
        {
            _configRepository = configRepository;
            _auditoriaRepository = auditoriaRepository;
        }

        public async Task<ApiGenericResult<AssistenteConfigResult>> ObterAsync(int orgId)
        {
            var result = new ApiGenericResult<AssistenteConfigResult>();

            var config = await _configRepository.ObterAsync(orgId);
            if (config == null)
            {
                config = new AssistenteConfigResult
                {
                    NomeAssistente = "Assistente",
                    MensagemBoasVindas = "Olá! Como posso ajudar?",
                    AcoesRapidas = new List<string>(),
                    LimiarSimilaridadeChamadoPercent = 35,
                    RagTopK = 8,
                    RagSimilaridadeMinima = 0.22
                };
            }

            config.Ops = new AssistenteOpsResult
            {
                OpenAiConfigurada = !string.IsNullOrEmpty(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("OPENAI_API_KEY")),
                EvolutionApiConfigurada = !string.IsNullOrEmpty(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("EVOLUTION_API_BASE_URL")),
                EvolutionWebhookSecretConfigurada = !string.IsNullOrEmpty(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente("EVOLUTION_WEBHOOK_SECRET"))
            };

            result.Retorno = config;
            return result;
        }

        public async Task<ApiGenericResult<AssistenteConfigResult>> AtualizarAsync(AtualizarAssistenteConfigInput input, string codigoInternoColaborador, string nomeColaborador, int orgId)
        {
            var result = new ApiGenericResult<AssistenteConfigResult>();

            if (input == null)
                throw new ArgumentException("Dados são obrigatórios.");

            if (input.NomeAssistente != null && (input.NomeAssistente.Trim().Length < 1 || input.NomeAssistente.Trim().Length > 120))
                throw new ArgumentException("NomeAssistente deve ter entre 1 e 120 caracteres.");

            if (input.MensagemBoasVindas != null && (input.MensagemBoasVindas.Trim().Length < 1 || input.MensagemBoasVindas.Trim().Length > 2000))
                throw new ArgumentException("MensagemBoasVindas deve ter entre 1 e 2.000 caracteres.");

            if (input.AcoesRapidas != null)
            {
                if (input.AcoesRapidas.Count > 10)
                    throw new ArgumentException("Máximo de 10 ações rápidas.");
                if (input.AcoesRapidas.Any(a => string.IsNullOrWhiteSpace(a) || a.Trim().Length > 200))
                    throw new ArgumentException("Cada ação rápida deve ter entre 1 e 200 caracteres.");
            }

            if (input.LimiarSimilaridadeChamadoPercent.HasValue &&
                (input.LimiarSimilaridadeChamadoPercent < 0 || input.LimiarSimilaridadeChamadoPercent > 100))
                throw new ArgumentException("LimiarSimilaridadeChamadoPercent deve ser entre 0 e 100.");

            if (input.RagTopK.HasValue && (input.RagTopK < 1 || input.RagTopK > 20))
                throw new ArgumentException("RagTopK deve ser entre 1 e 20.");

            if (input.RagSimilaridadeMinima.HasValue && (input.RagSimilaridadeMinima < 0 || input.RagSimilaridadeMinima > 1))
                throw new ArgumentException("RagSimilaridadeMinima deve ser entre 0 e 1.");

            if (input.InstrucaoSistemaExtra != null && input.InstrucaoSistemaExtra.Length > 2000)
                throw new ArgumentException("InstrucaoSistemaExtra deve ter no máximo 2.000 caracteres.");

            var updateInput = new AssistenteConfigUpdateInput
            {
                NomeAssistente = input.NomeAssistente?.Trim(),
                MensagemBoasVindas = input.MensagemBoasVindas?.Trim(),
                AcoesRapidasJson = input.AcoesRapidas != null ? JsonSerializer.Serialize(input.AcoesRapidas) : null,
                LimiarSimilaridadeChamado = input.LimiarSimilaridadeChamadoPercent.HasValue
                    ? input.LimiarSimilaridadeChamadoPercent.Value / 100.0
                    : null,
                RagTopK = input.RagTopK,
                RagSimilaridadeMinima = input.RagSimilaridadeMinima,
                InstrucaoSistemaExtra = input.InstrucaoSistemaExtra,
                AlteradoPorCodigoInternoColaborador = codigoInternoColaborador
            };

            await _configRepository.SalvarAsync(updateInput, orgId);

            await _auditoriaRepository.InserirAsync(new AuditoriaInsertInput
            {
                CodigoInternoColaborador = codigoInternoColaborador,
                ActorLabel = nomeColaborador ?? "Sistema",
                Acao = "settings.assistant_updated",
                Detalhe = "Configurações do assistente atualizadas",
                Payload = JsonSerializer.Serialize(input),
                TipoEntidade = "assistente_config",
                EntidadeId = null
            }, orgId);

            return await ObterAsync(orgId);
        }
    }
}
