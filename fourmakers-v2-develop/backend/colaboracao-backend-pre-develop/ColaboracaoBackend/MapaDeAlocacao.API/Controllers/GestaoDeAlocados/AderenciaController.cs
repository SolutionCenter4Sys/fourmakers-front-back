using System;
using System.Collections.Generic;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.Aderencia;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ApiClient.Domain;
using Colaboracao.Helper;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados.Aderencia;
using DataTransferObject.Domain.Vaga;

namespace MapaDeAlocacao.API.Controllers.GestaoDeAlocados
{
    [Authorize]
    [Route("api/GestaoDeAlocados/[controller]")]
    [HandleException]
    [ApiController]
    [LogAction]
    public class AderenciaController : ControllerBase
    {
        private readonly IAderenciaService _aderenciaService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public AderenciaController(IAderenciaService aderenciaService, IAspNetUser aspNetUser)
        {
            _aderenciaService = aderenciaService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("ListarAderenciasPerfil")]
        public async Task<ActionResult> ListarAderenciasPerfil([FromBody] ListaAderenciaParam listaAderenciaParam)
        {
            ApiGenericResult<System.Collections.Generic.List<ColaboradorAderenciaDTO>> aderencias = await _aderenciaService.ListarAderenciaPerfilGestorExterno(listaAderenciaParam.GestorExternoPerfilId, listaAderenciaParam.Filtro, listaAderenciaParam.Cursor, listaAderenciaParam.Limite, listaAderenciaParam.OrgId, _usuarioLogado.Cpf);
            return Ok(aderencias);
        }

        [HttpPost("ListarAderenciasColaborador")]
        public async Task<ActionResult> ListarAderenciasColaborador([FromBody] ListasPerfisAderentesParams listaAderenciaColaboradorParam)
        {
            ApiGenericResult<PerfisAderentesDTO> listarPerfilsaderentes = await _aderenciaService.ListarPerfisAderentesColaborador(listaAderenciaColaboradorParam, _usuarioLogado.Cpf);
            return Ok(listarPerfilsaderentes);
        }
        
        [HttpGet("ListarAderenciaAlocados")]
        public async Task<ActionResult<List<CalculoAderenciaComPerfilEColaboradorDTO>>> ListarAderenciaAlocados([FromQuery] int orgId)
        {
            if (_usuarioLogado.Token != VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO))
            {
                throw new AccessViolationException("Não autorizado para essa organização");
            }
            ApiGenericResult<List<CalculoAderenciaComPerfilEColaboradorDTO>> listarPerfilsaderentes = await _aderenciaService.ListarAderenciaAlocadosColabEPerfil(_usuarioLogado.Cpf, orgId);
            return Ok(listarPerfilsaderentes);
        }

        [HttpGet("ListarAderenciaAlocadosViaMatch")]
        public async Task<ActionResult<List<ListarCandidatosAderentesResult>>> ListarAderenciaAlocadosViaMatch(Guid perfilId, int orgId, int cursor = 0, int limite = 10)
        {
            var result = new ApiGenericResult<IEnumerable<ListarCandidatosAderentesResult>>();

            var listarPerfilsaderentes = await _aderenciaService.ListarAderenciaAlocadosViaMatch(_usuarioLogado.Cpf, perfilId, orgId, cursor, limite);
            return Ok(listarPerfilsaderentes);
        }
    }
}