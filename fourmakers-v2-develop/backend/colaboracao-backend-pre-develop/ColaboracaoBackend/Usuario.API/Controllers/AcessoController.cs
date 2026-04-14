using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper.Util;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Usuario.API.DTOs;
using Usuario.Domain.Interfaces.Services;

namespace Usuario.API
{
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class AcessoController : ControllerBase
    {
        private readonly IAcessoUsuarioService _acessoUsuarioService;
        private readonly IAspNetUser _aspNetUser;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AcessoController(IAspNetUser aspNetUser, IAcessoUsuarioService acessoUsuarioService, IHttpContextAccessor httpContextAccessor)
        {
            _acessoUsuarioService = acessoUsuarioService;
            _aspNetUser = aspNetUser;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("EnviaTokenAcessoEmail")]
        public async System.Threading.Tasks.Task<ActionResult<AcessoUsuarioResult>> EnviaTokenAcessoEmail([FromBody] EnviaTokenParam param)
        {
            try
            {
                var ret = await _acessoUsuarioService.EnviaTokenAcessoEmail(param.email, param.orgId, param.forceCodigoEmail);
                return Ok(new AcessoUsuarioResult
                {
                    Sucesso = true,
                    TipoAcesso = ret
                });
            }
            catch (ValidationException e)
            {
                return StatusCode(400, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = e.Message
                });
            }
            catch (Exception e)
            {
                return StatusCode(401, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Não autorizado"
                });
            }
        }

        [HttpPost("ValidaTokenAcessoEmail")]
        public async System.Threading.Tasks.Task<ActionResult<UsuarioResult>> ValidaTokenAcessoEmail([FromBody] ValidaTokenParam param)
        {
            try
            {
                var ret = await _acessoUsuarioService.ValidaTokenAcessoEmail(param.email, param.token, param.orgId);
                return Ok(ret);
            }
            catch (Exception)
            {
                return StatusCode(401, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Código de acesso inválido."
                });
            }
        }

        [HttpPost("EnviaTokenAcessoEmailSemOrg")]
        public async System.Threading.Tasks.Task<ActionResult<UsuarioAcessoSemOrgResult>> EnviaTokenAcessoSemOrgEmail([FromBody] EnviaTokenSemOrgParam param)
        {
            try
            {
                var ret = await _acessoUsuarioService.EnviaTokenAcessoEmailSemOrg(param.Email);
                return Ok(ret);
            }
            catch (ValidationException e)
            {
                return StatusCode(400, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = e.Message
                });
            }
            catch (Exception e)
            {
                return StatusCode(401, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Não autorizado"
                });
            }
        }

        [HttpGet("ObtemCodigoAcessoEmailQA")]
        public async System.Threading.Tasks.Task<ActionResult> ObtemCodigoAcessoEmailQA([FromQuery] string email, [FromQuery] int orgId)
        {
            try
            {
                var tokenSistema = AuthorizationUtil.ObterTokenBearer(_httpContextAccessor.HttpContext);
                var codigo = await _acessoUsuarioService.ObtemCodigoAcessoEmail(tokenSistema, email, orgId);
                return Ok(new { Sucesso = true, Codigo = codigo });
            }
            catch (ValidationException e)
            {
                return StatusCode(400, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = e.Message
                });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(401, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Não autorizado"
                });
            }
            catch (Exception)
            {
                return StatusCode(401, new StatusResult
                {
                    Sucesso = false,
                    Mensagem = "Não autorizado"
                });
            }
        }
    }
}