using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Vagas;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.API.Controllers
{
    public partial class VagaController
    {
        [HttpGet("ListarSolicitantes")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarSolicitantes()
        {
            try
            {
                return await _vagaService.ListarSolicitantes();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarAprovadores")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarAprovadores()
        {
            try
            {
                return await _vagaService.ListarAprovadores();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarTermometroVagas")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarTermometroVagas()
        {
            try
            {
                return await _vagaService.ListarTermometroVagas();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarStackPrincipal")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarStackPrincipal()
        {
            try
            {
                return await _vagaService.ListarStackPrincipal();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarConfiguracaoMaquina")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarConfiguracaoMaquina(string idContaCrm, int hardskillId)
        {
            try
            {
                return await _vagaService.ListarConfiguracaoMaquina(idContaCrm, hardskillId);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarTipoVaga")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarTipoVaga()
        {
            try
            {
                return await _vagaService.ListarTipoVaga();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarDuracaoContrato")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarDuracaoContrato()
        {
            try
            {
                return await _vagaService.ListarDuracaoContrato();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarCargos")]
        public async Task<ActionResult<ApiGenericResult<List<CargoDropdownItemDTO>>>> ListarCargos()
        {
            try
            {
                return await _vagaService.ListarCargos();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarTipoContratacao")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarTipoContratacao()
        {
            try
            {
                return await _vagaService.ListarTipoContratacao();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarCargaHoraria")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarCargaHoraria()
        {
            try
            {
                return await _vagaService.ListarCargaHoraria();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarLocalTrabalho")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarLocalTrabalho()
        {
            try
            {
                return await _vagaService.ListarLocalTrabalho();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ListarUnidadesSRS")]
        public async Task<ActionResult<ApiGenericResult<List<DropDownItemDTO>>>> ListarUnidadesSRS()
        {
            try
            {
                return await _vagaService.ListarUnidadesSRS();
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiGenericResult<List<DropDownItemDTO>> { Sucesso = false, Mensagem = e.Message });
            }
        }
    }
}