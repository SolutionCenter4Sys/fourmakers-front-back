using Colaboracao.Core;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.MapaDemografico;
using DataTransferObject.Domain.Usuario;

namespace MapaDeAlocacao.API.Controllers.GestaoDeAlocados
{
    [Route("api/GestaoDeAlocados/[controller]")]
    [ApiController]
    [HandleException]
    [Authorize]
    [LogAction]
    public class MapaDemograficoController : ControllerBase
    {
        private readonly IMapaDemograficoService _mapaDemograficoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public MapaDemograficoController(IMapaDemograficoService mapaDemograficoService, IAspNetUser aspNetUser)
        {
            _mapaDemograficoService = mapaDemograficoService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("BuscarBancoTalentosOrgId")]
        public async Task<ActionResult<List<MapaDemograficoDTO>>> BuscarBancoTalentosOrgId(int? orgID = null, string talento = null)
        {
            ApiGenericResult<List<MapaDemograficoDTO>> result = await _mapaDemograficoService.BuscarBancoTalentosOrgId(_usuarioLogado.Cpf, orgID, talento);

            return Ok(result);
        }

        [HttpGet("BuscarBancoTalentosCodColaborador")]
        public async Task<ActionResult<List<MapaDemograficoDTO>>> BuscarBancoTalentosCodColaborador(string codColaborador)
        {
            ApiGenericResult<List<MapaDemograficoDTO>> result = await _mapaDemograficoService.BuscarBancoTalentosCodColaborador(codColaborador);

            return Ok(result);
        }
        
        [HttpGet("ListarSumarioPcds")]
        public async Task<ActionResult<ApiGenericResult<List<MapaDemograficoPcdSumarioDTO>>>> ListarSumarioPcds()
        {
            var resultado = await _mapaDemograficoService.ListarSumarioPcdsAsync(_usuarioLogado.OrgId);

            return Ok(resultado);
        }
    }
}
