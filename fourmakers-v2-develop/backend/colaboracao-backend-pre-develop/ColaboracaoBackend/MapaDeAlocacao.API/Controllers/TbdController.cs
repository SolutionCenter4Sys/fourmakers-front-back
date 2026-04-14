using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.Tbd;
using DataTransferObject.Domain.Usuario;
using MapaDeAlocacao.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace MapaDeAlocacao.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class TbdController : Controller
    {
        private readonly ITbdService _tbdService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public TbdController(IAspNetUser aspNetUser, ITbdService tbdService)
        {
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _tbdService = tbdService;
        }

        [HttpGet("ListarTbd")]
        public async Task<ActionResult<IEnumerable<TbdAlocacaoDTO>>> ListarTbd(string codGestor)
        {
            var listarAlocacoes = await _tbdService.ListarTbd(codGestor, _usuarioLogado.OrgId);
            return Ok(listarAlocacoes);
        }

        [HttpGet("ObterTbdPorCodigo")]
        public async Task<ActionResult<TbdAlocacaoDTO>> ObterTbdPorCodigo(int codTbd)
        {
            var listarAlocacoes = await _tbdService.ObterTbdPorCodigo(_usuarioLogado.OrgId, codTbd);

            return Ok(listarAlocacoes);
        }
        [HttpPost("InserirTbd")]
        public async Task<ActionResult> InserirTbd([FromBody] TbdAlocacaoParam param)
        {
            var result = await _tbdService.InserirTbd(param, _usuarioLogado.OrgId);

            return Ok(result);
        }

        [HttpPut("AtualizarTbd")]
        public async Task<ActionResult> AtualizarTbd([FromBody] TbdAlocacaoParam param)
        {
            var result = await _tbdService.AtualizarTbd(param, _usuarioLogado.OrgId);

            return Ok(result);
        }

        [HttpDelete("DeletarTbd")]
        public async Task<ActionResult<ApiGenericResult<StatusResult>>> DeletarTbd(int codTbd)
        {
            var ret = new ApiGenericResult<StatusResult>();

            ret.Retorno = await _tbdService.DeletarTbd(codTbd, _usuarioLogado.OrgId);
            ret.Mensagem = "Tbd excluído com sucesso.";
            return Ok(ret);
        }
    }
}