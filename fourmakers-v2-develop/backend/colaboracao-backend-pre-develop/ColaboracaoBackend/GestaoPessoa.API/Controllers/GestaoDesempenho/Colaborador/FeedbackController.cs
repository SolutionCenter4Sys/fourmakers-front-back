using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;
using GestaoPessoa.Domain.Interfaces.GestaoDesempenho.Colaborador;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestaoPessoa.API.Controllers.GestaoDesempenho.Colaborador
{
    [Authorize]
    [Route("api/GestaoPessoa/GestaoDesempenho/Colaborador/[controller]")]
    [HandleException]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IGestaoDesempenhoColaboradorService _gestaoDesempenhoColaboradorService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public FeedbackController(
            IGestaoDesempenhoColaboradorService gestaoDesempenhoColaboradorService,
            IAspNetUser aspNetUser)
        {
            _gestaoDesempenhoColaboradorService = gestaoDesempenhoColaboradorService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("InserirVisualizacao/{feedbackId}")]
        public async Task<ActionResult> InserirVisualizacaoFeedback(string feedbackId)
        {
            var resultado = await _gestaoDesempenhoColaboradorService.InserirVisualizacaoFeedbackAsync(
                feedbackId,
                _usuarioLogado.Cpf
            );

            return Ok(resultado);
        }

    }
}
