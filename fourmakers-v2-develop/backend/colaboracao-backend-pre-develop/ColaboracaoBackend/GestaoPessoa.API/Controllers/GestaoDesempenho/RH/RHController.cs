using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.RH;
using DataTransferObject.Domain.Usuario;
using GestaoPessoa.Domain.Interfaces.GestaoDesempenho.RH;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestaoPessoa.API.Controllers.GestaoDesempenho.RH
{
    [Authorize]
    [Route("api/GestaoPessoa/GestaoDesempenho/[controller]")]
    [HandleException]
    [ApiController]
    public class RHController : ControllerBase
    {
        private readonly IGestaoDesempenhoRHService _gestaoDesempenhoRHService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public RHController(
            IGestaoDesempenhoRHService gestaoDesempenhoRHService,
            IAspNetUser aspNetUser)
        {
            _gestaoDesempenhoRHService = gestaoDesempenhoRHService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ObterDashboard")]
        public async Task<ActionResult> ObterDashboard()
        {
            var resultado = await _gestaoDesempenhoRHService.ObterDashboardRHAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }

        [HttpGet("ListaColaboradores")]
        public async Task<ActionResult> ListaColaboradores(
            [FromQuery] bool? semFeedback,
            [FromQuery] bool? semOneOnOne,
            [FromQuery] bool? semOneOnOneDias,
            [FromQuery] bool? semFeedbackDias)
        {
            var filtros = new ListaColaboradoresRequestDTO
            {
                SemFeedback = semFeedback,
                SemOneOnOne = semOneOnOne,
                SemOneOnOneDias = semOneOnOneDias,
                SemFeedbackDias = semFeedbackDias
            };

            var resultado = await _gestaoDesempenhoRHService.ObterListaColaboradoresAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                filtros
            );

            return Ok(resultado);
        }

        [HttpPost("InserirParametrizacao")]
        public async Task<ActionResult> InserirParametrizacao([FromBody] InserirParametrizacaoRequestDTO request)
        {
            var resultado = await _gestaoDesempenhoRHService.InserirParametrizacaoAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );

            return Ok(resultado);
        }


        [HttpGet("ObterParametrizacao")]
        public async Task<ActionResult> ObterParametrizacao()
        {
            var resultado = await _gestaoDesempenhoRHService.ObterParametrizacaoPorOrgAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }
    }
}
