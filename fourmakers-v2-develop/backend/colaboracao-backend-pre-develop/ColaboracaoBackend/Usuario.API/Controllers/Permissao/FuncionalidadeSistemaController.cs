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
    public class FuncionalidadeSistemaController : ControllerBase
    {
        private readonly IFuncionalidadeSistemaService _funcionalidadeSistemaService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public FuncionalidadeSistemaController(IAspNetUser aspNetUser,
                                                IFuncionalidadeSistemaService funcionalidadeSistemaService)
        {
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _funcionalidadeSistemaService = funcionalidadeSistemaService;
        }

        [HttpGet("ListarFuncionalidadesSistema")]
        public async Task<ActionResult> ListarFuncionalidadesSistema()
        {
            var listaFuncionalidadeSistema = await _funcionalidadeSistemaService.ListarFuncionalidadesSistema(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(new ApiGenericResult<IEnumerable<FuncionalidadeSistemaDTO>> { Retorno = listaFuncionalidadeSistema });
        }

        [HttpGet("ListarPessoasPorFuncionalidadeSistema")]
        public async Task<ActionResult> ListarPessoasPorFuncionalidadeSistema([FromQuery] int? funcionalidadeId = null)
        {
            var listaPessoas = await _funcionalidadeSistemaService.ListarPessoasPorFuncionalidadeSistema(_usuarioLogado.Cpf, _usuarioLogado.OrgId, funcionalidadeId);
            return Ok(new ApiGenericResult<IEnumerable<PessoaFuncionalidadeSistemaDTO>> { Retorno = listaPessoas });
        }
    }
}