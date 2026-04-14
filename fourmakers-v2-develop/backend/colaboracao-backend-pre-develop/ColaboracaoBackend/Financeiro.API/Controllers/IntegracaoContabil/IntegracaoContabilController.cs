using Financeiro.Domain.Interfaces.IntegracaoContabil;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using DataTransferObject.Domain.Util.Enum;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.IntegracaoContabil
{
    [Authorize]
    [Route("api/Financeiro/[controller]")]
    [ApiController]
    [LogAction]
    public class IntegracaoContabilController : ControllerBase
    {
        private readonly IIntegracaoContabilService _integracaoContabilService;
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly IStringLocalizer<FourmakersCoreMessage> _stringLocalizerFourmakersCore;

        public IntegracaoContabilController(
            IIntegracaoContabilService integracaoContabilService,
            IAspNetUser aspNetUser,
            IStringLocalizer<FourmakersCoreMessage> stringLocalizerFourmakersCore)
        {
            _integracaoContabilService = integracaoContabilService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _stringLocalizerFourmakersCore = stringLocalizerFourmakersCore;
        }

        [HttpGet("GerarRemessaContabilMensal")]
        public async Task<IActionResult> GerarRemessaContabilMensal(string cnpj, string competencia)
        {
            var result = new ApiGenericResult<FileContentResult>();

            try
            {
                var fileResult = await _integracaoContabilService.GerarRemessaContabilMensalFolhaPontoERubricaAsync(
                    cnpj,
                    competencia,
                    _usuarioLogado.Cpf,
                    (int)EnumORG.ROYAL_9);//_usuarioLogado.OrgId); -- somente pra Royal hoje

                if (!fileResult.Sucesso)
                {
                    result.Mensagem = fileResult.Mensagem;
                    result.Sucesso = false;
                    return Ok(result);
                }

                result.Retorno = File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName, true);
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return Unauthorized(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

    }
}