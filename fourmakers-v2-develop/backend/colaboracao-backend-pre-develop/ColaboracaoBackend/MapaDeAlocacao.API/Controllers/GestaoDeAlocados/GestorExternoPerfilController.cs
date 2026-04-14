using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfil;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.GestorExternoPerfilSkill;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados.GestorExternoPerfilSkill;

namespace MapaDeAlocacao.API.Controllers.GestaoDeAlocados
{
    [Authorize]
    [Route("api/GestaoDeAlocados/[controller]")]
    [HandleException]
    [ApiController]
    [LogAction]
    public class GestorExternoPerfilController : ControllerBase
    {
        private readonly IGestorExternoPerfilService _gestorExternoPerfilService;
        private readonly IGestorExternoPerfilSkillService _gestorExternoPerfilSkillService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public GestorExternoPerfilController(IGestorExternoPerfilService gestorExternoPerfilService, IAspNetUser aspNetUser, IGestorExternoPerfilSkillService gestorExternoPerfilSkillService)
        {
            _gestorExternoPerfilService = gestorExternoPerfilService;
            _gestorExternoPerfilSkillService = gestorExternoPerfilSkillService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarGestorExternoPerfil")]
        public async Task<ActionResult> ListarGestorExternoPerfil()
        {
            var perfis = await _gestorExternoPerfilService.ListarGestorExternoPerfis(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfis);
        }

        [HttpGet("ObterGestorExternoPerfilPorId")]
        public async Task<ActionResult> ObterGestorExternoPerfilPorId(Guid id)
        {
            var perfil = await _gestorExternoPerfilService.ObterGestorExternoPerfilPorId(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfil);
        }

        [HttpPost("InserirGestorExternoPerfil")]
        public async Task<ActionResult> InserirGestorExternoPerfil([FromBody] GestorExternoPerfilInput param)
        {
            var result = await _gestorExternoPerfilService.InserirGestorExternoPerfil(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("AtualizarGestorExternoPerfil")]
        public async Task<ActionResult> AtualizarGestorExternoPerfil(Guid id, [FromBody] GestorExternoPerfilInput param)
        {
            var result = await _gestorExternoPerfilService.AtualizarGestorExternoPerfil(param, id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpDelete("DeletarGestorExternoPerfil")]
        public async Task<ActionResult> DeletarGestorExternoPerfil(Guid id)
        {
            var result = await _gestorExternoPerfilService.DeletarGestorExternoPerfil(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (!result.Sucesso)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("CadastrarPerfisLegado")]
        public async Task<ActionResult> CadastrarPerfisLegado([FromBody] CadastrarPerfisLegado param)
        {
            await _gestorExternoPerfilService.CadastrarPerfisLegado(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok();
        }
        
        [HttpGet("ListarSkillsPerfilGestorExternoPorId")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<GestorExternoPerfilSkillResult>>>> ListarSkillsPerfilGestorExternoPorId([FromQuery] Guid perfilId)
        {
            var result = await _gestorExternoPerfilSkillService.ListarSkillsPerfilGestorExternoPorId(perfilId);
            return Ok(result);
        }
    }
}