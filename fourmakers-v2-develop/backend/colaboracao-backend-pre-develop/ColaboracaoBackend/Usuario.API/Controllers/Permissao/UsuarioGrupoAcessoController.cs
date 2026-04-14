using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Usuario.Permissao;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Usuario.Domain.Interfaces.Services.Permissao;

namespace Usuario.API.Controllers.PermissaoAcesso
{
    [ApiController]
    [HandleException]
    [Route("api/Usuario/Permissao/[controller]")]
    [Authorize]
    [LogAction]
    public class UsuarioGrupoAcessoController : ControllerBase
    {
        private readonly IUsuarioGrupoAcessoService _usuarioGrupoAcessoService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public UsuarioGrupoAcessoController(IAspNetUser aspNetUser,
                                        IUsuarioGrupoAcessoService usuarioGrupoAcessoService)
        {
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _usuarioGrupoAcessoService = usuarioGrupoAcessoService;
        }

        [HttpGet("ObterGruposAcessoPorUsuario")]
        public async Task<ActionResult> ObterGruposAcessoPorUsuario(int usuarioId)
        {
            var listaUsuarioGrupoAcesso = await _usuarioGrupoAcessoService.ObterGruposAcessoPorUsuario(usuarioId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(new ApiGenericResult<UsuarioGrupoAcessoDTO> { Retorno = listaUsuarioGrupoAcesso });
        }

        [HttpGet("ObterGruposAcessoFuncionalidadesSistemaPorUsuario")]
        public async Task<ActionResult> ObterGruposAcessoFuncionalidadesSistemaPorUsuario(int usuarioId)
        {
            var listaUsuarioGrupoAcessoFuncionalidadeSistema = await _usuarioGrupoAcessoService.ObterGruposAcessoFuncionalidadesSistemaPorUsuario(usuarioId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(new ApiGenericResult<UsuarioGrupoAcessoFuncionalidadeSistemaDTO> { Retorno = listaUsuarioGrupoAcessoFuncionalidadeSistema });
        }

        [HttpPost("AdicionarUsuarioGrupoAcesso")]
        public async Task<ActionResult> AdicionarUsuarioGrupoAcesso(int usuarioId, int grupoAcessoId)
        {
            await _usuarioGrupoAcessoService.AdicionarUsuarioGrupoAcesso(usuarioId, grupoAcessoId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(new ApiGenericResult { Mensagem = "Usuário adicionado ao grupo de acesso com sucesso." });
        }

        [HttpPost("RemoverUsuarioGrupoAcesso")]
        public async Task<ActionResult> RemoverUsuarioGrupoAcesso(int usuarioId, int grupoAcessoId)
        {
            await _usuarioGrupoAcessoService.RemoverUsuarioGrupoAcesso(usuarioId, grupoAcessoId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(new ApiGenericResult { Mensagem = "Usuário removido do grupo de acesso com sucesso." });
        }
    }
}