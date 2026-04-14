using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Core.Domain.Labs;
using DataTransferObject.Domain.Labs.MatchSemantico;
using DataTransferObject.Domain.Log;
using Labs.Domain.Interfaces;
using Logs.Infra.Attributes;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Labs.Domain.Impl
{
    /// <summary>
    /// Serviço de domínio que orquestra a chamada ao Match Semântico (GCP).
    /// </summary>
    [LogDomainClass]
    public class MatchSemanticoService : IMatchSemanticoService
    {
        private readonly IMatchSemanticoClient _matchSemanticoClient;
        private readonly ILabsLogMatchSemanticoRepository _logMatchSemanticoRepository;
        private readonly ILogCore _log;

        public MatchSemanticoService(
            IMatchSemanticoClient matchSemanticoClient,
            ILabsLogMatchSemanticoRepository logMatchSemanticoRepository,
            ILogCore log)
        {
            _matchSemanticoClient = matchSemanticoClient;
            _logMatchSemanticoRepository = logMatchSemanticoRepository;
            _log = log;
        }

        public async Task<MatchSemanticoHydeResult> BuscarMelhoresCandidatosAsync(MatchSemanticoHydeRequest request, int orgId, string? codigoInternoColaborador)
        {
            var result = await _matchSemanticoClient.BuscarMelhoresCandidatosAsync(request);

            try
            {
                var objetoRequest = JsonSerializer.Serialize(request);
                var objetoResponse = JsonSerializer.Serialize(result);
                var idLog = await _logMatchSemanticoRepository.InserirAsync(orgId, codigoInternoColaborador, objetoRequest, objetoResponse);
                result.IdLogMatchSemantico = idLog;
            }
            catch (Exception ex)
            {
                _log.Log("Erro ao gravar log Match Semântico (BuscarMelhoresCandidatos)", LevelsEnum.Warning);
                _log.Log(ex.Message, LevelsEnum.Warning);
            }

            return result;
        }
    }
}
