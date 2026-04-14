using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.GestaoPessoa.GestaoDesempenho.Colaborador;
using DataTransferObject.Domain.Usuario;
using GestaoPessoa.Domain.Interfaces.GestaoDesempenho.Colaborador;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoPessoa.API.Controllers.GestaoDesempenho.Colaborador
{
    [Authorize]
    [Route("api/GestaoPessoa/GestaoDesempenho/[controller]")]
    [HandleException]
    [ApiController]
    public class ColaboradorController : ControllerBase
    {
        private readonly IGestaoDesempenhoColaboradorService _gestaoDesempenhoColaboradorService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public ColaboradorController(
            IGestaoDesempenhoColaboradorService gestaoDesempenhoColaboradorService,
            IAspNetUser aspNetUser)
        {
            _gestaoDesempenhoColaboradorService = gestaoDesempenhoColaboradorService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ObterDashboard")]
        public async Task<ActionResult> ObterDashboard()
        {
            var resultado = await _gestaoDesempenhoColaboradorService.ObterMeuDashboardAsync(
                _usuarioLogado.Cpf
            );

            return Ok(resultado);
        }

        [HttpPost("InserirPautaSugerida")]
        public async Task<ActionResult> InserirPautaSugerida([FromBody] InserirPautaSugeridaColaboradorRequestDTO request)
        {
            var resultado = await _gestaoDesempenhoColaboradorService.InserirPautaSugeridaColaboradorAsync(
                _usuarioLogado.Cpf,
                request
            );

            return Ok(resultado);
        }
    }
}
