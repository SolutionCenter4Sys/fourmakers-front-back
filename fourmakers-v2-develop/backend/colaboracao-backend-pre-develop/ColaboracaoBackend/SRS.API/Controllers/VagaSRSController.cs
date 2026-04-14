using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Vagas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SRS.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using DataTransferObject.Domain.VagasSRS;
using Logs.Infra.Attributes;

namespace SRS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [LogAction]
    public class VagaSRSController : ControllerBase
    {
        private readonly IVagaSRSService _vagaSRSService;
        private readonly IAspNetUser _aspNetUser;
        public VagaSRSController(IVagaSRSService vagaSRSService, IAspNetUser aspNetUser)
        {
            _vagaSRSService = vagaSRSService;
            _aspNetUser = aspNetUser;
        }

        [HttpGet("ListarSolicitantes")]
        public ActionResult<ApiGenericResult<List<DropDownItemDTO>>> ListarSolicitantes()
        {
            var ret = new ApiGenericResult<List<DropDownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarSolicitantes();
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarAprovadores")]
        public ActionResult<ApiGenericResult<List<DropDownItemDTO>>> ListarAprovadores()
        {
            var ret = new ApiGenericResult<List<DropDownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarAprovadores();
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarTermometroVagas")]
        public ActionResult<ApiGenericResult<List<DropDownItemDTO>>> ListarTermometroVagas()
        {
            var ret = new ApiGenericResult<List<DropDownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarTermometroVagas();
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarStackPrincipal")]
        public ActionResult<ApiGenericResult<List<DropDownItemDTO>>> ListarStackPrincipal()
        {
            var ret = new ApiGenericResult<List<DropDownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarStackPrincipal();
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarConfiguracaoMaquina")]
        public ActionResult<ApiGenericResult<List<DropDownItemDTO>>> ListarConfiguracaoMaquina(string idContaCrm, int hardskillId)
        {
            var ret = new ApiGenericResult<List<DropDownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarConfiguracaoMaquina(idContaCrm, hardskillId);
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarTipoVaga")]
        public ActionResult<ApiGenericResult<List<DropDownItemDTO>>> ListarTipoVaga()
        {
            var ret = new ApiGenericResult<List<DropDownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarTipoVaga();
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarDuracaoContrato")]
        public ActionResult<ApiGenericResult<List<DropDownItemDTO>>> ListarDuracaoContrato()
        {
            var ret = new ApiGenericResult<List<DropDownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarDuracaoContrato();
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarCargos")]
        public ActionResult<ApiGenericResult<List<CargoDropdownItemDTO>>> ListarCargos()
        {
            var ret = new ApiGenericResult<List<CargoDropdownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarCargos();
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarTipoContratacao")]
        public ActionResult<ApiGenericResult<List<DropDownItemDTO>>> ListarTipoContratacao()
        {
            var ret = new ApiGenericResult<List<DropDownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarTipoContratacao();
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarCargaHoraria")]
        public ActionResult<ApiGenericResult<List<DropDownItemDTO>>> ListarCargaHoraria()
        {
            var ret = new ApiGenericResult<List<DropDownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarCargaHoraria();
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarLocalTrabalho")]
        public ActionResult<ApiGenericResult<List<DropDownItemDTO>>> ListarLocalTrabalho()
        {
            var ret = new ApiGenericResult<List<DropDownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarLocalTrabalho();
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarUnidadesSRS")]
        public ActionResult<ApiGenericResult<List<DropDownItemDTO>>> ListarUnidadesSRS()
        {
            var ret = new ApiGenericResult<List<DropDownItemDTO>>();
            try
            {
                ret.Retorno = _vagaSRSService.ListarUnidadesSRS();
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }
        
        [HttpGet("ListarVagasParaSemanticaComSkill")]
        public ActionResult<ApiGenericResult<List<JobOrderSemanticaDTO>>> ListarVagasParaSemanticaComSkill(int cursor, int limite)
        {
            var ret = new ApiGenericResult<List<JobOrderSemanticaDTO>>();
            try
            {
                ValidacaoUtil.ObrigaCursorLimite(cursor, limite);
                ret.Retorno = _vagaSRSService.ListarVagasParaSemanticaComSkill(cursor, limite);
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (ApplicationException e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return BadRequest(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }
    }
}