using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Marketing.Comunicacao.IA;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.IA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketing.API.Controllers.Comunicacao.IA
{
    [Authorize]
    [Route("api/Marketing/Comunicacao/[controller]")]
    [HandleException]
    [ApiController]
    public class IAController : ControllerBase
    {
        private readonly IComunicacaoIAService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public IAController(
            IComunicacaoIAService service,
            IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("Assistente")]
        public async Task<ActionResult> Assistente([FromBody] AssistenteRequestDTO request)
        {
            var resultado = await _service.AssistenteAsync(request);

            return Ok(resultado);
        }
    }
}
