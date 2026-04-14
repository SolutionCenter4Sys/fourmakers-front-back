using Colaboracao.Core.Interfaces;
using CRM.Domain.Interfaces.Services;
using DataTransferObject.Domain.CRM;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CRM.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class CRMController : Controller
    {
        private readonly ICRMService _CRMService;
        private readonly IAspNetUser _aspNetUser;

        public CRMController(ICRMService CRMService, IAspNetUser aspNetUser)
        {
            _CRMService = CRMService;
            _aspNetUser = aspNetUser;
        }

        [HttpGet("BuscarContatoResponsavel")]
        public async Task<ActionResult<CRMContatoResponsavelOutputDTO>> BuscarContatoResponsavel(string crm)
        {
            var ret = new CRMContatoResponsavelOutputDTO();
            try
            {
                return await _CRMService.BuscarContatoResponsavel(crm, _aspNetUser.GetUsuarioLogado().Token);
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
                var ret = _CRMService.GetClientesCrm();
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
                var ret = _CRMService.GetCotacoesByCrmId(crmId);
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
                var ret = _CRMService.GetCotacoesLastTwoYearsByAccountId(accountId);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao obter as cotações por Account ID: " + ex.Message);
            }
        }
    }
}