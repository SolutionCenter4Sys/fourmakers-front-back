using ApiClient.Domain.Interfaces;
using Colaboracao.Core;
using Core.Domain.Labs;
using DataTransferObject.Domain.Labs;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Vaga;
using Labs.Domain.Interfaces;
using Logs.Infra;
using Logs.Infra.Attributes;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Labs.Domain.Impl
{
    /// <summary>
    /// Serviço de domínio que delega ao MatchClient (ExtrairPerfilDeUmPrompt) e grava log em tb_labs_log_extractor_extract_vaga quando contexto informado.
    /// </summary>
    [LogDomainClass]
    public class ExtractorService : IExtractorService
    {
        private readonly IMatchClient _matchClient;
        private readonly ILabsLogExtractorExtractVagaRepository _logExtractorExtractVagaRepository;
        private readonly ILogCore _log;

        public ExtractorService(IMatchClient matchClient, ILabsLogExtractorExtractVagaRepository logExtractorExtractVagaRepository, ILogCore log)
        {
            _matchClient = matchClient;
            _logExtractorExtractVagaRepository = logExtractorExtractVagaRepository;
            _log = log;
        }

        public async Task<ExtrairPerfilDeUmPromptResponse> ExtrairPerfilDeUmPrompt(ExtrairPerfilDeUmPromptRequest request, ExtrairPerfilLogContext? logContext = null)
        {
            var response = await _matchClient.ExtrairPerfilDeUmPrompt(request);

            if (logContext != null)
            {
                try
                {
                    await _logExtractorExtractVagaRepository.InserirAsync(
                        logContext.OrgId,
                        logContext.VagaId,
                        logContext.CodigoInternoColaborador,
                        JsonSerializer.Serialize(request),
                        JsonSerializer.Serialize(response));
                }
                catch (Exception ex)
                {
                    _log.Log("Erro ao gravar log Extractor ExtrairPerfilDeUmPrompt", LevelsEnum.Warning);
                    _log.Log(ex.Message, LevelsEnum.Warning);
                }
            }

            return response;
        }
    }
}
