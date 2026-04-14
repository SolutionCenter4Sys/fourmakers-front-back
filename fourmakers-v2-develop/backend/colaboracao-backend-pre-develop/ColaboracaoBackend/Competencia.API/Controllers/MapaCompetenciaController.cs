using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador.Cidadania;
using DataTransferObject.Domain.Colaborador.Vistos;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Competencia.MapaCompetencia;
using DataTransferObject.Domain.Formacao;
using DataTransferObject.Domain.Projeto;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Competencia.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/Competencia/[controller]")]
    [ApiController]
    [LogAction]
    public class MapaCompetenciaController : ControllerBase
    {
        private readonly IMapaCompetenciaService _mapaCompetenciaService;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public MapaCompetenciaController(IAspNetUser aspNetUser, IMapaCompetenciaService mapaCompetenciaService, IVerificaSeCpfESistemico verificaCpfSistemico)
        {
            _mapaCompetenciaService = mapaCompetenciaService;
            _verificaCpfSistemico = verificaCpfSistemico;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarCompetenciaSumario")]
        public async Task<ActionResult> ListarCompetenciaSumario(TipoCompetenciaSRSEnum enumTipoCompetencia)
        {
            var ret = new ApiGenericResult<List<CompetenciaSumarioDTO>>();
            ret.Retorno = await _mapaCompetenciaService.ListarCompetenciaSumario(enumTipoCompetencia, _usuarioLogado.OrgId);
            return Ok(ret);
        }

        [HttpGet("ListarFormacaoSumario")]
        public async Task<ActionResult> ListarFormacaoSumario()
        {
            var ret = new ApiGenericResult<List<FormacaoSumarioDTO>>();
            var orgId = _usuarioLogado.OrgId;
            ret.Retorno = await _mapaCompetenciaService.ListarFormacaoSumario(orgId);
            return Ok(ret);
        }

        [HttpGet("ListarLocalizacaoSumario")]
        public async Task<ActionResult> ListarLocalizacaoSumario()
        {
            var ret = new ApiGenericResult<List<LocalizacaoSumarioDTO>>();
            var orgId = _usuarioLogado.OrgId;
            ret.Retorno = await _mapaCompetenciaService.ListarLocalizacaoSumario(orgId);
            return Ok(ret);
        }

        [HttpGet("ListarCargoSumario")]
        public async Task<ActionResult> ListarCargoSumario()
        {
            var ret = new ApiGenericResult<List<CargoColaboradorSumario>>();
            var orgId = _usuarioLogado.OrgId;
            ret.Retorno = await _mapaCompetenciaService.ListarCargoSumario(orgId);
            return Ok(ret);
        }

        [HttpGet("ListarClienteSumario")]
        public async Task<ActionResult> ListarClienteSumario()
        {
            var ret = new ApiGenericResult<List<ClienteSumarioDTO>>();
            var orgId = _usuarioLogado.OrgId;
            ret.Retorno = await _mapaCompetenciaService.ListarClienteSumario(orgId);
            return Ok(ret);
        }

        [HttpGet("ListarCursosSumario")]
        public async Task<ActionResult<ApiGenericResult<EstatisticasCursoDTO>>> ListarCursosSumario()
        {
            var ret = new ApiGenericResult<List<EstatisticasCursoDTO>>();
            var orgId = _usuarioLogado.OrgId;
            ret.Retorno = await _mapaCompetenciaService.ListarEstatisticasCursos(orgId);
            return Ok(ret);
        }

        [HttpGet("ListarCidadaniasSumario")]
        public async Task<ActionResult<ApiGenericResult<EstatisticaCidadaniaDTO>>> ListarCidadaniasSumario()
        {
            var ret = new ApiGenericResult<List<EstatisticaCidadaniaDTO>>();
            var orgId = _usuarioLogado.OrgId;
            ret.Retorno = await _mapaCompetenciaService.ListarEstatisticasCidadanias(orgId);
            return Ok(ret);
        }

        [HttpGet("ListaColaboradoresSkillsDetalhes")]
        public async Task<ActionResult> ListaColaboradoresSkillsDetalhes()
        {
            var ret = new ApiGenericResult<List<ColaboradorSkillDetalheArrayDTO>>();
            var orgId = _usuarioLogado.OrgId;
            ret.Retorno = await _mapaCompetenciaService.ListaColaboradoresSkillsDetalhes(orgId);
            return Ok(ret);
        }

        [HttpGet("RelatorioColaboradoresSkillsDetalhes")]
        public async Task<ActionResult> RelatorioColaboradoresSkillsDetalhes()
        {
            var orgId = _usuarioLogado.OrgId;
            var fileResult = await _mapaCompetenciaService.RelatorioColaboradoresSkillsDetalhes(orgId);

            if (!fileResult.Sucesso)
            {
                return StatusCode(500, fileResult.Mensagem);
            }

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }

        [HttpGet("RelatorioColaboradoresSkillsTotalizador")]
        public async Task<ActionResult> RelatorioColaboradoresSkillsTotalizador()
        {
            var orgId = _usuarioLogado.OrgId;
            var fileResult = await _mapaCompetenciaService.RelatorioColaboradoresSkillsTotalizador(orgId);

            if (!fileResult.Sucesso)
            {
                return StatusCode(500, fileResult.Mensagem);
            }

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }

        [HttpGet("ListarVistosSumario")]
        public async Task<ActionResult<ApiGenericResult<EstatisticaVistoDTO>>> ListarVistosSumario()
        {
            var ret = new ApiGenericResult<List<EstatisticaVistoDTO>>();
            var orgId = _usuarioLogado.OrgId;
            ret.Retorno = await _mapaCompetenciaService.ListarEstatisticasVisto(orgId);
            return Ok(ret);
        }
    }
}