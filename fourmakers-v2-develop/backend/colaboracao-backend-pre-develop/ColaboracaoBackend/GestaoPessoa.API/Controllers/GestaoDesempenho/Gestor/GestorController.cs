using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using DataTransferObject.Domain.Usuario;
using GestaoPessoa.Domain.Interfaces.GestaoDesempenho.Gestor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestaoPessoa.API.Controllers.GestaoDesempenho.Gestor
{
    [Authorize]
    [Route("api/GestaoPessoa/GestaoDesempenho/[controller]")]
    [HandleException]
    [ApiController]
    public class GestorController : ControllerBase
    {
        private readonly IGestaoDesempenhoGestorService _gestaoDesempenhoGestorService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public GestorController(
            IGestaoDesempenhoGestorService gestaoDesempenhoGestorService,
            IAspNetUser aspNetUser)
        {
            _gestaoDesempenhoGestorService = gestaoDesempenhoGestorService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ObterDashboard")]
        public async Task<ActionResult> ObterDashboard()
        {
            var resultado = await _gestaoDesempenhoGestorService.ObterDashboardGestorAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        [HttpGet("MeusColaboradores")]
        public async Task<ActionResult> MeusColaboradores(
            [FromQuery] bool? semFeedback,
            [FromQuery] bool? semOneOnOne,
            [FromQuery] bool? semOneOnOneAcimaDeParametroDias,
            [FromQuery] bool? semFeedbackAcimaDeParametroDias)
        {
            var filtros = new MeusColaboradoresRequestDTO
            {
                SemFeedback = semFeedback,
                SemOneOnOne = semOneOnOne,
                SemOneOnOneAcimaDeParametroDias = semOneOnOneAcimaDeParametroDias,
                SemFeedbackAcimaDeParametroDias = semFeedbackAcimaDeParametroDias
            };

            var resultado = await _gestaoDesempenhoGestorService.ObterMeusColaboradoresAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                filtros
            );

            return Ok(resultado);
        }

        [HttpPost("InserirPautaSugerida")]
        public async Task<ActionResult> InserirPautaSugerida([FromBody] InserirPautaSugeridaGestorRequestDTO request)
        {
            var resultado = await _gestaoDesempenhoGestorService.InserirPautaSugeridaGestorAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );

            return Ok(resultado);
        }

        [HttpPost("AtualizarRegistroCriticoOneOnOne/{oneOnOneId}/{registroCritico}")]
        public async Task<ActionResult> DescriticarOneOnOneAsync(string oneOnOneId, bool registroCritico)
        {
            var resultado = await _gestaoDesempenhoGestorService.AtualizarRegistroCriticoOneOnOneAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                oneOnOneId,
                registroCritico
            );

            return Ok(resultado);
        }
    }
}
