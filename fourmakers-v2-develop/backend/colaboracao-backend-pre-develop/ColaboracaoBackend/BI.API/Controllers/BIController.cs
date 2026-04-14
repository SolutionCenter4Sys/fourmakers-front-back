using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BI.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BI.API.Controllers;

[Route("api/BI")]
[ApiController]
[LogAction]
public class BIController : ControllerBase
{
    private readonly IBIService _bIService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BIController(IBIService bIService, IHttpContextAccessor httpContextAccessor)
    {
        _bIService = bIService;
        _httpContextAccessor = httpContextAccessor;
    }
    
    [HttpGet("RelatorioProjetosEAprovadores")]
    public async Task<IActionResult> RelatorioAprovadores()
    {
        try
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
            var result = await _bIService.RelatorioProjetosEAprovadores(tokenSistema);
            return Ok(result);
        }
        catch (AccessViolationException)
        {
            return StatusCode(401, "Not authorized");
        }
        catch (ValidationException e)
        {
            return StatusCode(400, e.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}