using BI.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BI.API
{
    [Route("api/BI/[controller]")]
    [ApiController]
    [LogAction]
    public class SRSBIController : ControllerBase
    {
        private readonly IBIService _bIService;
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;

        public SRSBIController(IBIService bIService, Microsoft.AspNetCore.Http.IHttpContextAccessor pHttpContextAccessor)
        {
            _bIService = bIService;
            _httpContextAccessor = pHttpContextAccessor;
        }

        [HttpGet("GetRelatorioCandidatos")]
        public async Task<IActionResult> GetRelatorioCandidatos()
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
                var colaboradores = await _bIService.GetRelatorioCandidate(tokenSistema);
                return Ok(colaboradores);
            }
            catch (AccessViolationException)
            {
                return StatusCode(401, "Not authorized");
            }
            catch (ValidationException e)
            {
                return StatusCode(400, e.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}