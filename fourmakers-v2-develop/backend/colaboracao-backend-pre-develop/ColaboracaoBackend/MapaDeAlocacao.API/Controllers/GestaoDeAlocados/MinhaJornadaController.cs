using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.Match;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Impl.GestaoDeAlocados;
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
    public class MinhaJornadaController : ControllerBase
    {
        private readonly IMinhaJornadaService _minhaJornadaService;
        private readonly IAspNetUser _aspNetUser;

        public MinhaJornadaController(IMinhaJornadaService minhaJornadaService, IAspNetUser aspNetUser)
        {
            _minhaJornadaService = minhaJornadaService;
            _aspNetUser = aspNetUser;
        }

        [HttpGet("BuscarSkillColaborador")]
        public async Task<ActionResult<List<MinhaJornadaColaboradorDTO>>> BuscarSkillColaborador(string codColaborador)
        {
            ApiGenericResult<List<MinhaJornadaColaboradorDTO>> result = await _minhaJornadaService.BuscarSkillColaborador(codColaborador);

            return Ok(result);
        }

        [HttpGet("BuscarSkillColaboradorAlocado")]
        public async Task<ActionResult<List<MinhaJornadaDTO>>> BuscarSkillColaboradorAlocado(string codColaborador, int orgId)
        {
            ApiGenericResult<List<MinhaJornadaDTO>> result = await _minhaJornadaService.BuscarSkillColaboradorAlocado(codColaborador, orgId);

            return Ok(result);
        }

        [HttpPost("InserirSugestao")]
        public async Task<ActionResult<ApiGenericResult<SugestaoSkilleHistoricoOrdemResponseDTO>>> InserirSugestao([FromBody] SugestaoParamDTO param, [FromQuery] bool minhaJornada = false)
            => Ok(await _minhaJornadaService.InserirSugestao(param, minhaJornada, _aspNetUser.GetUsuarioLogado().Cpf));

        [HttpPost("AprovarRejeitarSugestao")]
        public async Task<ActionResult<ApiGenericResult<List<SugestaoHistoricoDTO>>>> AprovarRejeitarSugestao([FromBody] SugestaoHistoricoParamDTO param)
            => Ok(await _minhaJornadaService.AprovarRejeitarSugestao(param));

        [HttpPut("AtualizarSugestao")]
        public async Task<ActionResult<ApiGenericResult<SugestaoDTO>>> AtualizarSugestao([FromBody] SugestaoAtualizacaoParamDTO param)
            => Ok(await _minhaJornadaService.AtualizarSugestao(param));

        [HttpDelete("DeletarSugestao")]
        public async Task<ActionResult<ApiGenericResult<bool>>> DeletarSugestao(string sugestaoId)
            => Ok(await _minhaJornadaService.DeletarSugestao(sugestaoId));

        [HttpGet("BuscarSugestaoPorId")]
        public async Task<ActionResult<ApiGenericResult<SugestaoDTO>>> BuscarSugestaoPorId(string sugestaoId)
            => Ok(await _minhaJornadaService.BuscarSugestaoPorId(sugestaoId));

        [HttpGet("BuscarSugestaoPorCodColaboradorOuAdm")]
        public async Task<ActionResult<ApiGenericResult<List<SugestaoSkilleHistoricoResponseDTO>>>> BuscarSugestaoPorCodColaboradorOuAdm(string codInternoColaborador = "", string codInternoGestorAdm = "", string perfilId = "")
            => Ok(await _minhaJornadaService.BuscarSugestaoPorCodColaboradorOuAdm(codInternoColaborador, codInternoGestorAdm, perfilId));


        [HttpGet("BuscarHistoricoPorId")]
        public async Task<ActionResult<ApiGenericResult<List<SugestaoHistoricoDTO>>>> BuscarHistoricoPorId(string historicoId)
            => Ok(await _minhaJornadaService.BuscarHistoricoPorId(historicoId));

        [HttpGet("BuscarSugestaoHistoricoPorId")]
        public async Task<ActionResult<ApiGenericResult<SugestaoResponseDTO>>> BuscarSugestaoHistoricoPorId(string sugestaoId)
            => Ok(await _minhaJornadaService.BuscarSugestaoHistoricoPorId(sugestaoId));

        [HttpGet("CalcularAderenciaDoColaboradorAoPerfil")]
        public async Task<ActionResult<ApiGenericResult<CandidatosMatchResponse>>> CalcularAderenciaDoColaboradorAoPerfil(Guid perfilId, string codigoInternoColaborador)
        {
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();
            ApiGenericResult<CandidatosMatchResponse> result = await _minhaJornadaService.CalcularAderenciaDoColaboradorAoPerfil(perfilId, codigoInternoColaborador, usuarioLogado.OrgId, usuarioLogado.Cpf);
            return Ok(result);
        }

    }
}
