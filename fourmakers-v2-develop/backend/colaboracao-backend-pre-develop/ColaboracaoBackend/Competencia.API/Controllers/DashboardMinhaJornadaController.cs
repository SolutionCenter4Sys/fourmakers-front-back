using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Competencia.Domain.Interfaces.Services.DashboardMinhaJornada;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia.DashboardMinhaJornada;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Competencia.API.Controllers
{
    [Authorize]
    [HandleException]
    [ApiController]
    [Route("api/Competencia/[controller]")]
    [LogAction]
    public class DashboardMinhaJornadaController : ControllerBase
    {
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IDashboardMinhaJornadaService _dashboardMinhaJornadaService;

        public DashboardMinhaJornadaController(IAspNetUser aspNetUser, IDashboardMinhaJornadaService dashboardMinhaJornadaService)
        {
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _dashboardMinhaJornadaService = dashboardMinhaJornadaService;
        }

        [HttpGet("BigNumbers")]
        public async Task<ActionResult<ApiGenericResult<BigNumbersDashboardMinhaJornada>>> BigNumbers()
            => Ok(await _dashboardMinhaJornadaService.BigNumbers(_usuarioLogado.OrgId));

        [HttpGet("TopDez")]
        public async Task<ActionResult<ApiGenericResult<TopDezDashboardMinhaJornada>>> TopDez()
            => Ok(await _dashboardMinhaJornadaService.TopDez(_usuarioLogado.OrgId));

        [HttpGet("LogDetalhado")]
        public async Task<ActionResult<ApiGenericResult<List<LogDetalhadoDashboardMinhaJornada>>>> LogDetalhado([FromQuery] int limit, [FromQuery] int cursor)
            => Ok(await _dashboardMinhaJornadaService.LogDetalhado(limit, cursor, _usuarioLogado.OrgId));

        [HttpGet("RelatorioLogDetalhado")]
        public async Task<IActionResult> RelatorioLogDetalhado(DateTime? dataInicio, DateTime? dataFim)
        {
            var fim = dataFim ?? DateTime.Now;
            var inicio = dataInicio ?? fim.AddDays(-30);

            var fileResult = await _dashboardMinhaJornadaService.RelatorioLogDetalhado(inicio, fim, _usuarioLogado.OrgId);

            if (!fileResult.Sucesso)
                return NoContent();

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }
    }
}
