using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Usuario.Permissao;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services.Permissao;

namespace Usuario.API.Controllers.PermissaoAcesso
{
    [ApiController]
    [HandleException]
    [Route("api/Usuario/Permissao/[controller]")]
    [Authorize]
    [LogAction]
    public class GrupoAcessoController : ControllerBase
    {
        private readonly IGrupoAcessoService _grupoAcessoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public GrupoAcessoController(IAspNetUser aspNetUser,
                                IGrupoAcessoService grupoAcessoService)
        {
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _grupoAcessoService = grupoAcessoService;
        }

        [HttpGet("ListarGruposAcesso")]
        public async Task<ActionResult> ListarGruposAcesso()
        {
            var listaGrupoAcesso = await _grupoAcessoService.ListarGruposAcesso(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(new ApiGenericResult<IEnumerable<GrupoAcessoDTO>> { Retorno = listaGrupoAcesso });
        }

        [HttpGet("ListarGruposAcessoFuncionalidadesSistema")]
        public async Task<ActionResult> ListarGruposAcessoFuncionalidadesSistema()
        {
            var listaGrupoAcessoFuncionalidadeSistema = await _grupoAcessoService.ListarGruposAcessoFuncionalidadesSistema(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(new ApiGenericResult<IEnumerable<GrupoAcessoFuncionalidadeSistemaDTO>> { Retorno = listaGrupoAcessoFuncionalidadeSistema });
        }

        [HttpPost("AdicionarGrupoAcessoFuncionalidadeSistema")]
        public async Task<ActionResult> AdicionarGrupoAcessoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId)
        {
            await _grupoAcessoService.AdicionarGrupoAcessoFuncionalidadeSistema(grupoAcessoId, funcionalidadeSistemaId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(new ApiGenericResult { Mensagem = "Funcionalidade associada ao grupo de acesso com sucesso." });
        }

        [HttpPost("RemoverGrupoAcessoFuncionalidadeSistema")]
        public async Task<ActionResult> RemoverGrupoAcessoFuncionalidadeSistema(int grupoAcessoId, int funcionalidadeSistemaId)
        {
            await _grupoAcessoService.RemoverGrupoAcessoFuncionalidadeSistema(grupoAcessoId, funcionalidadeSistemaId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(new ApiGenericResult { Mensagem = "Funcionalidade removida do grupo de acesso com sucesso." });
        }

        [HttpPost("CriarGrupoAcesso")]
        public async Task<ActionResult> CriarGrupoAcesso([FromBody] CriarGrupoAcessoInput input)
        {
            var grupoAcesso = await _grupoAcessoService.CriarGrupoAcesso(input, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(new ApiGenericResult<GrupoAcessoDTO> 
            { 
                Retorno = grupoAcesso,
                Mensagem = "Grupo de acesso criado com sucesso." 
            });
        }

        [HttpPut("EditarGrupoAcesso")]
        public async Task<ActionResult> EditarGrupoAcesso([FromBody] EditarGrupoAcessoInput input)
        {
            await _grupoAcessoService.AtualizarGrupoAcesso(input, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(new ApiGenericResult 
            { 
                Mensagem = "Grupo de acesso atualizado com sucesso." 
            });
        }

        [HttpGet("ListarPessoasPorGrupoAcesso")]
        public async Task<ActionResult> ListarPessoasPorGrupoAcesso([FromQuery] int? grupoId = null)
        {
            var listaPessoas = await _grupoAcessoService.ListarPessoasPorGrupoAcesso(_usuarioLogado.Cpf, _usuarioLogado.OrgId, grupoId);
            return Ok(new ApiGenericResult<IEnumerable<PessoaGrupoAcessoDTO>> { Retorno = listaPessoas });
        }
    }
}