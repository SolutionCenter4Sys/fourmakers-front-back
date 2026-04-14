using System;
using System.Threading.Tasks;
using BI.Domain.Interfaces.Services;
using Colaboracao.Core;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BI.API.Controllers;

[Route("api/BI/[controller]")]
[HandleException]
[ApiController]
[LogAction]
public class MapaAlocacaoBIController : ControllerBase
{
    private readonly IMapaAlocacaoBIService _mapaAlocacaoBiService;
    private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;

    public MapaAlocacaoBIController(IMapaAlocacaoBIService mapaAlocacaoBiService, IHttpContextAccessor httpContextAccessor)
    {
        _mapaAlocacaoBiService = mapaAlocacaoBiService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("ListarColaboradorCompleto")]
    public async Task<IActionResult> ListarColaboradorCompleto()
    {
       
            string tokenSistema;
            try
            {
                tokenSistema = _httpContextAccessor.HttpContext.Request.Headers["authorization"].ToString()["Bearer ".Length..].Trim();
            }
            catch (Exception)
            {
                throw new AccessViolationException();
            }
            var colaboradores = await _mapaAlocacaoBiService.GetAderenciaAlocados(tokenSistema);
            return Ok(colaboradores);
    }
}