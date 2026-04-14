using BI.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BI.API.Controllers
{
    [Route("api/BI/[controller]")]
    [ApiController]
    [LogAction]
    public class ApontamentoController : ControllerBase
    {
        private readonly IBIService _bIService;
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;

        public ApontamentoController(IBIService bIService, Microsoft.AspNetCore.Http.IHttpContextAccessor pHttpContextAccessor)
        {
            _bIService = bIService;
            _httpContextAccessor = pHttpContextAccessor;
        }

        [HttpGet("RelatorioApontamentos")]
        public async Task<IActionResult> RelatorioApontamentos()
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
                var fileResult = await _bIService.GeraRelatorioApontamento(tokenSistema);
                return Ok(fileResult);
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

        [HttpGet("RelatorioApontamentosRecentes")]
        public async Task<IActionResult> RelatorioApontamentosRecentes()
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
                var fileResult = await _bIService.GeraRelatorioApontamentoRecentes(tokenSistema);
                return Ok(fileResult);
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

        [HttpGet("RelatorioAlocacoes")]
        public async Task<IActionResult> RelatorioAlocacao()
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
                var fileResult = await _bIService.GeraRelatorioAlocacao(tokenSistema);
                return Ok(fileResult);
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

        [HttpGet("RelatorioAlocacoesTodos")]
        public async Task<IActionResult> RelatorioAlocacaoTodos()
        {
            try
            {
                string tokenSistema;
                try
                {
                    tokenSistema =
                        _httpContextAccessor.HttpContext.Request.Headers["authorization"].ToString()["Bearer ".Length..]
                            .Trim();
                }
                catch (Exception)
                {
                    throw new AccessViolationException();
                }

                var fileResult = await _bIService.GeraAlocacaoTodos(tokenSistema);
                return Ok(fileResult);
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