using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Notificacao;
using DataTransferObject.Domain.Usuario;
using Firebase.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Firebase.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class NotificacaoController : ControllerBase
    {
        private readonly UsuarioLogadoDTO _usuarioLogado;
        private readonly INotificacaoService _notificacaoService;

        public NotificacaoController(IAspNetUser aspNetUser, INotificacaoService notificacaoService)
        {
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _notificacaoService = notificacaoService;
        }

        [HttpGet("ListarNotificacoesColaborador")]
        public async Task<ActionResult<ApiGenericResult<List<NotificacaoDTO>>>> ListarNotificacoesColaborador([FromQuery] bool apenasNaoLidas = false)
        {
            var ret = new ApiGenericResult<List<NotificacaoDTO>>();
            ret.Retorno = await _notificacaoService.ListarNotificacoesColaborador(_usuarioLogado.Cpf, _usuarioLogado.OrgId, apenasNaoLidas);
            return Ok(ret);
        }

        [HttpGet("ContarNotificacoesNaoLidasColaborador")]
        public async Task<ActionResult<ApiGenericResult<int>>> ContarNotificacoesNaoLidasColaborador()
        {
            var ret = new ApiGenericResult<int>();
            ret.Retorno = await _notificacaoService.ContarNotificacoesNaoLidasColaborador(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(ret);
        }

        [HttpPost("MarcarNotificacoesComoLidasColaborador")]
        public async Task<ActionResult<StatusResult>> MarcarNotificacoesComoLidasColaborador()
        {
            var ret = new StatusResult();
            ret.Sucesso = await _notificacaoService.MarcarNotificacoesComoLidasColaborador(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(ret);
        }
    }
}