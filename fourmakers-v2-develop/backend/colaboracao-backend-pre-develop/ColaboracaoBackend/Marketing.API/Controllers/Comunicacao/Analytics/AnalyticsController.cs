using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Marketing.Comunicacao.Analytics;
using DataTransferObject.Domain.Usuario;
using Marketing.Domain.Interfaces.Comunicacao.Analytics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Marketing.API.Controllers.Comunicacao.Analytics
{
    [Authorize]
    [Route("api/Marketing/Comunicacao/[controller]")]
    [HandleException]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IComunicacaoAnalyticsService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public AnalyticsController(IComunicacaoAnalyticsService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("Resumo")]
        public async Task<ActionResult> ObterResumo([FromBody] AnalyticsResumoRequestDTO request)
        {
            var resultado = await _service.ObterResumoAsync(_usuarioLogado.OrgId, request);
            return Ok(resultado);
        }

        [HttpPost("ResumoComunicadosOficiais")]
        public async Task<ActionResult> ObterResumoComunicadosOficiais([FromBody] AnalyticsResumoRequestDTO request)
        {
            var resultado = await _service.ObterResumoComunicadosOficiaisAsync(_usuarioLogado.OrgId, request);
            return Ok(resultado);
        }

        [HttpPost("PostsPorComunidade")]
        public async Task<ActionResult> ObterPostsPorComunidade([FromBody] PostsPorComunidadeRequestDTO request)
        {
            var resultado = await _service.ObterPostsPorComunidadeAsync(_usuarioLogado.OrgId, request);
            return Ok(resultado);
        }
    }
}
