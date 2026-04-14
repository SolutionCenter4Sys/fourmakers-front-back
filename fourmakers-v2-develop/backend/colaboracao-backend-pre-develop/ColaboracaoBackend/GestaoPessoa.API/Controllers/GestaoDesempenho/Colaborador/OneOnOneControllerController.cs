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
    public class OneOnOneController : ControllerBase
    {
        private readonly IGestaoDesempenhoColaboradorService _gestaoDesempenhoColaboradorService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public OneOnOneController(
            IGestaoDesempenhoColaboradorService gestaoDesempenhoColaboradorService,
            IAspNetUser aspNetUser)
        {
            _gestaoDesempenhoColaboradorService = gestaoDesempenhoColaboradorService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("InserirVisualizacao/{oneOnOneId}")]
        public async Task<ActionResult> InserirVisualizacaoOneOnOne(string oneOnOneId)
        {
            var resultado = await _gestaoDesempenhoColaboradorService.InserirVisualizacaoOneOnOneAsync(
                oneOnOneId,
                _usuarioLogado.Cpf
            );

            return Ok(resultado);
        }
    }
}
