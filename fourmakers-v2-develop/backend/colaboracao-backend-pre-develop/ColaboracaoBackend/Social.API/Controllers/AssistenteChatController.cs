using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Domain.Interfaces.AtendimentoFourmakers;

namespace Social.API.Controllers
{
    [HandleException]
    [Route("api/Social/[controller]")]
    [ApiController]
    [Authorize]
    [LogAction]
    public class AssistenteChatController : ControllerBase
    {
        private readonly IAssistenteChatService _service;
        private readonly DataTransferObject.Domain.Usuario.UsuarioLogadoDTO _usuarioLogado;

        public AssistenteChatController(IAssistenteChatService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("Conversar")]
        public async Task<ActionResult> Conversar([FromBody] ConversarInput input)
        {
            var result = await _service.ConversarAsync(input, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPost("Feedback")]
        public async Task<ActionResult> Feedback([FromBody] FeedbackChatInput input)
        {
            var result = await _service.RegistrarFeedbackAsync(input, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("ListarLogs")]
        public async Task<ActionResult> ListarLogs([FromQuery] FiltroLogsInput filtro)
        {
            var result = await _service.ListarLogsAsync(filtro ?? new FiltroLogsInput(), _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("FeedbackStats")]
        public async Task<ActionResult> FeedbackStats()
        {
            var result = await _service.ObterFeedbackStatsAsync(_usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("MetricasMensal")]
        public async Task<ActionResult> MetricasMensal()
        {
            var result = await _service.ObterMetricasMensalAsync(_usuarioLogado.OrgId);
            return Ok(result);
        }
    }
}
