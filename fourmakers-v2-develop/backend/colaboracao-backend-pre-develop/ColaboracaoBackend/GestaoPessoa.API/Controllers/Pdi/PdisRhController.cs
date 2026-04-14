using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;
using GestaoPessoa.Domain.Interfaces.Services.Pdi;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestaoPessoa.API.Controllers.Pdi
{
    /// <summary>Listagem de PDIs (RH / Minha Equipe) na org do usuário logado; escopo de diretoria quando houver restrição.</summary>
    [Authorize]
    [ApiController]
    [Route("api/GestaoPessoa/Pdi/[controller]")]
    [HandleException]
    [LogAction]
    public class PdisRhController : ControllerBase
    {
        private readonly IPdisRhService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public PdisRhController(IPdisRhService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        /// <summary>
        /// Lista PDIs na organização do token. Opcional: <c>colaboradorId</c> (codigo_interno),
        /// <c>gestorCodigoInterno</c> (apenas subordinados na hierarquia desse gestor).
        /// Paginação: <c>pagina</c>, <c>tamanhoPagina</c> (máx. 100).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> ListarPorOrg(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 10,
            [FromQuery] string colaboradorId = null,
            [FromQuery] string gestorCodigoInterno = null)
        {
            var result = await _service.ListarPdisRhAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                pagina,
                tamanhoPagina,
                colaboradorId,
                gestorCodigoInterno);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Contagens por status (nao_iniciado, em_analise, em_andamento, finalizados, cancelados)
        /// no mesmo escopo da listagem.</summary>
        [HttpGet("ObterBigNumbers")]
        public async Task<ActionResult> ObterBigNumbers(
            [FromQuery] string colaboradorId = null,
            [FromQuery] string gestorCodigoInterno = null)
        {
            var result = await _service.ObterBigNumbersAsync(
                _usuarioLogado.Cpf,
                _usuarioLogado.OrgId,
                colaboradorId,
                gestorCodigoInterno);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }
    }
}
