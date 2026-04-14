using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Social;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Social.API.Controllers
{
    [HandleException]
    [Route("api/Social/[controller]")]
    [ApiController]
    [Authorize]
    [LogAction]
    public class EncontrosBigNumbersController : ControllerBase
    {
        private readonly IEncontrosBigNumbersService _encontrosBigNumbersService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public EncontrosBigNumbersController(IEncontrosBigNumbersService encontrosBigNumbersService, IAspNetUser aspNetUser)
        {
            _encontrosBigNumbersService = encontrosBigNumbersService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ObterBigNumbers")]
        public async Task<ActionResult<ApiGenericResult<EncontrosBigNumbers>>> ObterBigNumbers([FromQuery] EncontrosBigNumbersParam param)
            => Ok(await _encontrosBigNumbersService.ObterBigNumbers(param, _usuarioLogado.OrgId));

        [HttpGet("ObterBigNumbersCategoria")]
        public async Task<ActionResult<ApiGenericResult<List<EncontrosBigNumbersCategoria>>>> ObterBigNumbersCategoria([FromQuery] EncontrosBigNumbersParam param)
            => Ok(await _encontrosBigNumbersService.ObterBigNumbersCategoria(param, _usuarioLogado.OrgId));

        [HttpGet("ObterBigNumbersObjetivo")]
        public async Task<ActionResult<ApiGenericResult<List<EncontrosBigNumbersObjetivo>>>> ObterBigNumbersObjetivo([FromQuery] EncontrosBigNumbersParam param)
            => Ok(await _encontrosBigNumbersService.ObterBigNumbersObjetivo(param, _usuarioLogado.OrgId));

        [HttpGet("AgendaRealizadaDetalhe")]
        public async Task<ActionResult<ApiGenericResult<List<AgendaRealizadaDetalhe>>>> AgendaRealizadaDetalhe([FromQuery] EncontrosBigNumbersParam param)
            => Ok(await _encontrosBigNumbersService.AgendaRealizadaDetalhe(param, _usuarioLogado.OrgId));

        [HttpGet("AgendaSemInteracaoDetalhe")]
        public async Task<ActionResult<ApiGenericResult<List<AgendaSemInteracaoDetalhe>>>> AgendaSemInteracaoDetalhe([FromQuery] EncontrosBigNumbersParam param)
            => Ok(await _encontrosBigNumbersService.AgendaSemInteracaoDetalhe(param, _usuarioLogado.OrgId));

        [HttpGet("ClientesImpactadosDetalhe")]
        public async Task<ActionResult<ApiGenericResult<List<ClientesImpactadosDetalhe>>>> ClientesImpactadosDetalhe([FromQuery] EncontrosBigNumbersParam param)
            => Ok(await _encontrosBigNumbersService.ClientesImpactadosDetalhe(param, _usuarioLogado.OrgId));

        [HttpGet("GestoresImpactadosDetalhe")]
        public async Task<ActionResult<ApiGenericResult<List<GestoresImpactadosDetalhe>>>> GestoresImpactadosDetalhe([FromQuery] EncontrosBigNumbersParam param)
            => Ok(await _encontrosBigNumbersService.GestoresImpactadosDetalhe(param, _usuarioLogado.OrgId));

        [HttpGet("CategoriaComInteracaoDetalhe")]
        public async Task<ActionResult<ApiGenericResult<List<CategoriaComInteracaoDetalhe>>>> CategoriaComInteracaoDetalhe([FromQuery] EncontrosBigNumbersParam param)
            => Ok(await _encontrosBigNumbersService.CategoriaComInteracaoDetalhe(param, _usuarioLogado.OrgId));

        [HttpGet("ObjetivosAgendaDetalhe")]
        public async Task<ActionResult<ApiGenericResult<List<ObjetivosAgendaDetalhe>>>> ObjetivosAgendaDetalhe([FromQuery] string objetivoIds = null, [FromQuery] EncontrosBigNumbersParam param = null)
            => Ok(await _encontrosBigNumbersService.ObjetivosAgendaDetalhe(objetivoIds, param, _usuarioLogado.OrgId));

        [HttpGet("AcoesEmAtrasoDetalhe")]
        public async Task<ActionResult<ApiGenericResult<List<AcoesEmAtrasoDetalhe>>>> AcoesEmAtrasoDetalhe([FromQuery] EncontrosBigNumbersParam param)
            => Ok(await _encontrosBigNumbersService.AcoesEmAtrasoDetalhe(param, _usuarioLogado.OrgId));

        [HttpGet("CategoriaEmFocoDetalhe/{categoriaId}")]
        public async Task<ActionResult<ApiGenericResult<CategoriaEmFocoDetalhe>>> CategoriaEmFocoDetalhe(int categoriaId, [FromQuery] EncontrosBigNumbersParam param)
            => Ok(await _encontrosBigNumbersService.CategoriaEmFocoDetalhe(categoriaId, param, _usuarioLogado.OrgId));

        [HttpPost("AtualizaVersaoApp")]
        public async Task<ActionResult<ApiGenericResult<AgendaVersaoDTO>>> AtualizaVersaoApp([FromBody] AtualizaVersaoAppParam param)
            => Ok(await _encontrosBigNumbersService.AtualizaVersaoApp(param));

        [HttpGet("ListaVersaoApp")]
        public async Task<ActionResult<ApiGenericResult<AgendaVersaoDTO>>> ListaVersaoApp()
            => Ok(await _encontrosBigNumbersService.ListaVersaoApp());
    }
}
