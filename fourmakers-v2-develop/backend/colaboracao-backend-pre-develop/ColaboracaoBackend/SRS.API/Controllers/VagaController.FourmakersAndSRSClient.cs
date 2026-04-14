using Colaboracao.Helper;
using Colaboracao.Helper.Enum;
using Colaboracao.Helper.Util;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Util.Enum;
using DataTransferObject.Domain.Vaga;
using DataTransferObject.Domain.VagasSRS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SRS.API.Controllers
{
    public partial class VagaController
    {
        [HttpPost("CadastroVaga")]
        public async Task<ActionResult<ApiGenericResult<CriarVagasSRSParam>>> CadastroVaga(VagaFourmakersDTO vagaDTO)
        {
            return await VagaCadastroRedirecionamento(vagaDTO, CRUDEnum.Create);
        }

        [HttpPut("EditarVaga")]
        public async Task<ActionResult<ApiGenericResult<CriarVagasSRSParam>>> EditarVaga(int? idVaga, [FromBody] VagaFourmakersDTO vagaDTO)
        {
            vagaDTO.IdVaga = idVaga;
            return await VagaCadastroRedirecionamento(vagaDTO, CRUDEnum.Update);
        }

        private async Task<ActionResult<ApiGenericResult<CriarVagasSRSParam>>> VagaCadastroRedirecionamento(VagaFourmakersDTO vagaDTO, CRUDEnum cRUDEnum)
        {
            try
            {
                var retorno = await _vagaService.EditarCadastrarVaga(vagaDTO, cRUDEnum);
                return retorno;
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (ArgumentException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ObterVagaPorIdEGestorExternoPerfilId")]
        public async Task<ActionResult<ApiGenericResult<VagaFourmakersDTO>>> ObterVagaPorIdEGestorExternoPerfilId(int vagaId, Guid? gestorExternoPerfilId)
        {
            try
            {
                var retorno = await _vagaService.ObterVagaPorIdEGestorExternoPerfilId(vagaId.ToIntOuZero(), gestorExternoPerfilId);
                return retorno;
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (ArgumentException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
        }
        
        [HttpGet("ListarVagasSemantica")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiGenericResult<List<JobOrderSemanticaListagemDTO>>>> ListarVagasSemantica(int cursor, int limite)
        {
            try
            {
                var tokenSistema = AuthorizationUtil.ObterTokenBearer(_httpContextAccessor.HttpContext);
                _tokenSistemaService.GetOrgTokenSistemaWithValidatingOrgs(tokenSistema, EnumORG.FOURMAKERS_1);

                var retorno = await _vagaService.ListarVagasSemantica(cursor, limite);
                return retorno;
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (AccessViolationException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (ApplicationException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (ArgumentException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
        }
    }
}