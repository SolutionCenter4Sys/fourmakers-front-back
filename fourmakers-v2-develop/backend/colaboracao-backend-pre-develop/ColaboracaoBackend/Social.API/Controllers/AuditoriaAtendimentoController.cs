using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Social.AtendimentoFourmakers;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Domain.Interfaces.AtendimentoFourmakers;
using System;

namespace Social.API.Controllers
{
    [HandleException]
    [Route("api/Social/[controller]")]
    [ApiController]
    [Authorize]
    [LogAction]
    public class AuditoriaAtendimentoController : ControllerBase
    {
        private readonly IAuditoriaAtendimentoService _service;
        private readonly DataTransferObject.Domain.Usuario.UsuarioLogadoDTO _usuarioLogado;

        public AuditoriaAtendimentoController(IAuditoriaAtendimentoService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("Listar")]
        public async Task<ActionResult> Listar([FromQuery] FiltroAuditoriaInput filtro)
        {
            if (filtro?.Formato?.Equals("csv", StringComparison.OrdinalIgnoreCase) == true)
            {
                var csvBytes = await _service.ExportarCsvAsync(filtro, _usuarioLogado.OrgId);
                var fileName = $"auditoria-{DateTime.Now:yyyy-MM-dd}.csv";
                return File(csvBytes, "text/csv; charset=utf-8", fileName);
            }

            var result = await _service.ListarAsync(filtro ?? new FiltroAuditoriaInput(), _usuarioLogado.OrgId);
            return Ok(result);
        }
    }
}
