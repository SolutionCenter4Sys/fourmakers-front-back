using ColaboracaoBridge.Domain.Interfaces.Services;
using DataTransferObject.Domain.CRM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace ColaboracaoBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class CRMBridgeController : Controller
    {
        private readonly ICRMBridgeService _colaboracaoBridgeService;

        public CRMBridgeController(ICRMBridgeService colaboracaoBridgeService)
        {
            _colaboracaoBridgeService = colaboracaoBridgeService;
        }

        [HttpGet("BuscarContatoResponsavel")]
        public ActionResult<CRMContatoResponsavelOutputDTO> BuscarContatoResponsavel(string crm)
        {
            var ret = new CRMContatoResponsavelOutputDTO();
            try
            {
                ret = _colaboracaoBridgeService.BuscarContatoResponsavel(crm);
                return ret;
            }
            catch
            {
                return StatusCode(500, ret);
            }
        }

        [HttpGet("GetClientesCrm")]
        public IActionResult GetClientesCrm()
        {
            try
            {
                var ret = _colaboracaoBridgeService.GetClientesCrm();
                return Ok(ret);
            }
            catch
            {
                return StatusCode(500, "Erro ao obter os clientes do CRM.");
            }
        }

        [HttpGet("GetCotacoesByCrmId")]
        public IActionResult GetCotacoesByCrmId(string crmId)
        {
            try
            {
                var ret = _colaboracaoBridgeService.GetCotacoesByCrmId(crmId);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao obter as cotações por CRM ID: " + ex.Message);
            }
        }

        [HttpGet("GetCotacoesLastTwoYearsByAccountId")]
        public IActionResult GetCotacoesLastTwoYearsByAccountId(int accountId)
        {
            try
            {
                var ret = _colaboracaoBridgeService.GetCotacoesLastTwoYearsByAccountId(accountId);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao obter as cotações por Account ID: " + ex.Message);
            }
        }

        [HttpGet("GetClientesFourmakersCrm")]
        public async Task<IActionResult> GetClientesFourmakersCrm()
        {
            try
            {
                var ret = await _colaboracaoBridgeService.GetClientesFourmakersCrm();
                return Ok(ret);
            }
            catch
            {
                return StatusCode(500, "Erro ao obter os clientes do CRM.");
            }
        }

        [HttpPost("GetGestoresPorAccountNos")]
        public async Task<IActionResult> GetGestoresPorAccountNos([FromBody] IEnumerable<string> accountNos)
        {
            try
            {
                var ret = await _colaboracaoBridgeService.GetGestoresPorAccountNos(accountNos);
                return Ok(ret);
            }
            catch
            {
                return StatusCode(500, "Erro ao obter os gestores do CRM.");
            }
        }

        [HttpPost("GetCotacoesIncluindoAccountNoByQuoteNos")]
        public async Task<IActionResult> GetCotacoesIncluindoAccountNoByQuoteNos([FromBody] IEnumerable<string> quoteNos)
        {
            try
            {
                var ret = await _colaboracaoBridgeService.GetCotacoesIncluindoAccountNoByQuoteNos(quoteNos.ToList());
                return Ok(ret);
            }
            catch
            {
                return StatusCode(500, "Erro ao obter as cotações do CRM.");
            }
        }
    }
}