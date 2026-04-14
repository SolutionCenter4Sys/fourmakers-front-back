using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MapaDeAlocacao.API.Controllers.GestaoDeAlocados
{
    [Route("api/GestaoDeAlocados/[controller]")]
    [ApiController]
    [HandleException]
    [Authorize]
    [LogAction]
    public class MinhaEquipeController : ControllerBase
    {
        private readonly IMinhaEquipeService _minhaEquipeService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public MinhaEquipeController(IMinhaEquipeService minhaEquipeService, IAspNetUser aspNetUser)
        {
            _minhaEquipeService = minhaEquipeService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("BuscarLideradosPorGestor")]
        public async Task<ActionResult<List<MinhaEquipeDTO>>> BuscarLideradosPorGestor(string codGestorAdm, string perfilId, int orgId)
        {
            ApiGenericResult<List<MinhaEquipeDTO>> result = await _minhaEquipeService.BuscarLideradosPorGestor(codGestorAdm, perfilId, orgId);

            return Ok(result);
        }

        [HttpGet("ListaIndicadoresDosLiderados")]
        public async Task<ActionResult<List<GestorColaboradoresSkill>>> ListaIndicadoresDosLiderados(string codGestorAdm, string perfilId, int orgId)
        {
            ApiGenericResult<List<GestorColaboradoresSkill>> result = await _minhaEquipeService.ListaIndicadoresDosLiderados(codGestorAdm, perfilId, orgId);

            return Ok(result);
        }

        [HttpGet("ListaAderenciaDosLideradosPorGestor")]
        public async Task<ActionResult<List<GestorCandidatosMatchResponse>>> ListaAderenciaDosLideradosPorGestor(string codGestorAdm,string perfilId, int orgId)
        {
            ApiGenericResult<List<GestorCandidatosMatchResponse>> result = await _minhaEquipeService.ListaAderenciaDosLideradosPorGestor(codGestorAdm, perfilId, orgId );

            return Ok(result);
        }

        [HttpGet("ListaAderenciaDosLideradosPorGestorSemParamPerfil")]
        public async Task<ActionResult<List<GestorCandidatosMatchResponse>>> ListaAderenciaDosLideradosPorGestorSemParamPerfil(string codGestorAdm = "", string codGestorOper = "", int orgId = 2)
        {
            ApiGenericResult<List<GestorCandidatosMatchResponsePerfil>> result = await _minhaEquipeService.ListaAderenciaDosLideradosPorGestorSemParamPerfil(_usuarioLogado.Cpf, codGestorAdm, codGestorOper, orgId);

            return Ok(result);
        }

        [HttpGet("ListaAderenciaDoColaborador")]
        public async Task<ActionResult<List<GestorCandidatosMatchResponse>>> ListaAderenciaDoColaborador(string codColaborador, string perfilId, int orgId)
        {
            ApiGenericResult<List<GestorCandidatosMatchResponse>> result = await _minhaEquipeService.ListaAderenciaDoColaborador(codColaborador, perfilId, orgId);

            return Ok(result);
        }

        [HttpGet("ListaAderenciaDoColaboradorAdmOper")]
        public async Task<ActionResult<List<GestorCandidatosMatchResponse>>> ListaAderenciaDoColaboradorAdmOper(string codColaborador, int orgId)
        {
            ApiGenericResult<List<GestorCandidatosMatchResponse>> result = await _minhaEquipeService.ListaAderenciaDoColaboradorAdmOper(codColaborador, orgId);

            return Ok(result);
        }


        [HttpGet("ListaIndicadoresDosLideradosAdmOper")]
        public async Task<ActionResult<List<GestorColaboradoresSkillAdmOper>>> ListaIndicadoresDosLideradosAdmOper(int orgId, int limite, int cursor, string codGestorAdm = "", string codGestorOper = "", string codCliente = "")
        {
            ApiGenericResult<List<GestorColaboradoresSkillAdmOper>> result = await _minhaEquipeService.ListaIndicadoresDosLideradosAdmOper(_usuarioLogado.Cpf, orgId, limite, cursor, codGestorAdm, codGestorOper, codCliente);

            return Ok(result);
        }

        [HttpGet("TotalizacaoIndicadoresDosLideradosAdmOper")]
        public async Task<ActionResult<TotalizacaoIndicadoresPorGestorAdmOper>> TotalizacaoIndicadoresDosLideradosAdmOper(int orgId, string codGestorAdm = "", string codGestorOper = "")
        {
            ApiGenericResult<List<TotalizacaoIndicadoresPorGestorAdmOper>> result = await _minhaEquipeService.TotalizacaoIndicadoresDosLideradosAdmOper(_usuarioLogado.Cpf, orgId, codGestorAdm, codGestorOper);

            return Ok(result);
        }

    }
}
