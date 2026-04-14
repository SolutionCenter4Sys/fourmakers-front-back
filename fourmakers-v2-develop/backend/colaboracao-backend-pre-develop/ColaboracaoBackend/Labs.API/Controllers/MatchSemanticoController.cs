using System.Threading.Tasks;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Labs;
using DataTransferObject.Domain.Labs.MatchSemantico;
using Labs.Domain.Interfaces;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Labs.API.Controllers
{
    [Authorize]
    [HandleException]
    [LogAction]
    [Route("api/Labs/[controller]")]
    [ApiController]
    public class MatchSemanticoController : ControllerBase
    {
        private readonly IMatchSemanticoService _matchSemanticoService;
        private readonly ILabsFeedbackMatchSemanticoService _feedbackMatchSemanticoService;
        private readonly IAspNetUser _aspNetUser;

        public MatchSemanticoController(IMatchSemanticoService matchSemanticoService, ILabsFeedbackMatchSemanticoService feedbackMatchSemanticoService, IAspNetUser aspNetUser)
        {
            _matchSemanticoService = matchSemanticoService;
            _feedbackMatchSemanticoService = feedbackMatchSemanticoService;
            _aspNetUser = aspNetUser;
        }

        /// <summary>
        /// Encaminha a requisição para o endpoint best_candidates/hyde do Match Semântico (GCP).
        /// </summary>
        [HttpPost("BuscarMelhoresCandidatos")]
        public async Task<ActionResult<ApiGenericResult<object>>> BuscarMelhoresCandidatos([FromBody] MatchSemanticoHydeRequest request)
        {
            var usuario = _aspNetUser.GetUsuarioLogado();
            var orgId = usuario?.OrgId ?? 0;
            var codigoColaborador = !string.IsNullOrEmpty(usuario?.CodColaborador) ? usuario.CodColaborador : usuario?.Cpf;

            var result = await _matchSemanticoService.BuscarMelhoresCandidatosAsync(request, orgId, codigoColaborador);

            var apiResult = new ApiGenericResult<object>
            {
                Sucesso = result.Sucesso,
                Retorno = result,
                Mensagem = result.Mensagem
            };

            if (result.Sucesso)
                return Ok(apiResult);

            var statusCode = int.TryParse(result.HttpStatus, out var code) ? code : 502;
            return StatusCode(statusCode, apiResult);
        }

        /// <summary>
        /// Métricas da organização: gerações de match semântico e preferência nos feedbacks (RankCandidatesIds vs Match Semântico).
        /// </summary>
        [HttpGet("MetricasMatchSemantico")]
        public async Task<ActionResult<ApiGenericResult<MatchSemanticoMetricasDto>>> MetricasMatchSemantico()
        {
            var usuario = _aspNetUser.GetUsuarioLogado();

            var metricas = await _feedbackMatchSemanticoService.ObterMetricasAsync(usuario.OrgId);

            return Ok(new ApiGenericResult<MatchSemanticoMetricasDto>
            {
                Sucesso = true,
                Retorno = metricas
            });
        }

        /// <summary>
        /// Registra feedback do usuário: qual resultado de match preferiu (RankCandidatesIds vs Match Semântico).
        /// </summary>
        [HttpPost("RegistrarFeedback")]
        public async Task<ActionResult<ApiGenericResult<object>>> RegistrarFeedback([FromBody] FeedbackMatchSemanticoRequest request)
        {
            var usuario = _aspNetUser.GetUsuarioLogado();
            var orgId = usuario?.OrgId ?? 0;
            var codigoColaborador = !string.IsNullOrEmpty(usuario?.CodColaborador) ? usuario.CodColaborador : usuario?.Cpf;

            await _feedbackMatchSemanticoService.RegistrarFeedbackAsync(request, orgId, codigoColaborador);

            return Ok(new ApiGenericResult<object> { Sucesso = true, Mensagem = "Feedback registrado." });
        }
    }
}
