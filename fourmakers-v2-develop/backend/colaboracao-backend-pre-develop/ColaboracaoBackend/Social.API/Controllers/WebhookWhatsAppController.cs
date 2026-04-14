using Colaboracao.Core;
using DataTransferObject.Domain.Base;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Mvc;
using Social.Domain.Interfaces.AtendimentoFourmakers;
using System.Linq;

namespace Social.API.Controllers
{
    [HandleException]
    [Route("api/Social/[controller]")]
    [ApiController]
    [LogAction]
    public class WebhookWhatsAppController : ControllerBase
    {
        private readonly IWebhookWhatsAppService _service;
        private const int DefaultOrgId = 2;

        public WebhookWhatsAppController(IWebhookWhatsAppService service)
        {
            _service = service;
        }

        [HttpGet("HealthCheck")]
        public ActionResult HealthCheck()
        {
            var result = new ApiGenericResult<object>();
            result.Retorno = new { ok = true, service = "atendimento-fourmakers-whatsapp-webhook" };
            return Ok(result);
        }

        [HttpPost("Receber")]
        public async Task<ActionResult> Receber([FromBody] object payload)
        {
            var secret = ExtrairWebhookSecret();
            var result = await _service.ProcessarWebhookAsync(payload, secret, DefaultOrgId);
            return Ok(result);
        }

        private string ExtrairWebhookSecret()
        {
            if (Request.Headers.TryGetValue("x-webhook-secret", out var headerSecret))
                return headerSecret.FirstOrDefault();

            if (Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var bearer = authHeader.FirstOrDefault();
                if (bearer != null && bearer.StartsWith("Bearer "))
                    return bearer["Bearer ".Length..];
            }

            if (Request.Query.TryGetValue("secret", out var querySecret))
                return querySecret.FirstOrDefault();

            return null;
        }
    }
}
