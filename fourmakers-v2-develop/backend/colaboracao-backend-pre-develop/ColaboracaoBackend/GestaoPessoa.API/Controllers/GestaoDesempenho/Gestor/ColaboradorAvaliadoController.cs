using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Gestor;
using DataTransferObject.Domain.Usuario;
using GestaoPessoa.Domain.Interfaces.GestaoDesempenho.Gestor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GestaoPessoa.API.Controllers.GestaoDesempenho.Gestor
{
    [Authorize]
    [Route("api/GestaoPessoa/GestaoDesempenho/Gestor/[controller]")]
    [HandleException]
    [ApiController]
    public class ColaboradorAvaliadoController : ControllerBase
    {
        private readonly IGestaoDesempenhoGestorService _gestaoDesempenhoGestorService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public ColaboradorAvaliadoController
(
            IGestaoDesempenhoGestorService gestaoDesempenhoGestorService,
            IAspNetUser aspNetUser)
        {
            _gestaoDesempenhoGestorService = gestaoDesempenhoGestorService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("InserirFeedback")]
        public async Task<ActionResult> InserirFeedback([FromBody] InserirFeedbackRequestDTO request)
        {
            var resultado = await _gestaoDesempenhoGestorService.InserirFeedbackAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );

            return Ok(resultado);
        }

        [HttpPost("InserirOneOnOne")]
        public async Task<ActionResult> InserirOneOnOne([FromBody] InserirOneOnOneRequestDTO request)
        {
            var resultado = await _gestaoDesempenhoGestorService.InserirOneOnOneAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                request
            );

            return Ok(resultado);
        }

        [HttpGet("ObterDashboard/{codigoInternoColaboradorAvaliado}")]
        public async Task<ActionResult> ObterDashboardColaborador(string codigoInternoColaboradorAvaliado)
        {
            var resultado = await _gestaoDesempenhoGestorService.ObterDashboardColaboradorAsync(
                _usuarioLogado.Cpf,
                codigoInternoColaboradorAvaliado,
                _usuarioLogado.OrgId
            );

            return Ok(resultado);
        }
    }
}
