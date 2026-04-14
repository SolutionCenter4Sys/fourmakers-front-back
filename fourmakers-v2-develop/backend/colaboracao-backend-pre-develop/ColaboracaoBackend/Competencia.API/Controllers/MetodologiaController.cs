using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Competencia.API.DTOs;
using Competencia.Domain.Interfaces.Services.Metodologia;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Metodologia;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Competencia.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/Competencia/[controller]")]
    [ApiController]
    [LogAction]
    public class MetodologiaController : Controller
    {
        private IMetodologiasService _metodologiaService;

        private readonly UsuarioLogadoDTO _usuarioLogado;

        public MetodologiaController(IMetodologiasService metodologiaService, IAspNetUser aspNetUser)
        {
            _metodologiaService = metodologiaService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("GetGroupMetodologiaById")]
        public async Task<ActionResult<List<CompetenciaGroupDTO>>> GetGroupMetodologiaById(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return BadRequest("Informe ao menos um id.");

            // separa por vírgula → converte para long
            var listaIds = ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.Parse(x.Trim()))
                .ToList();

            var competencias = await _metodologiaService.GetGroupMetodologiaByIds(listaIds);
            return Ok(competencias);
        }

        [HttpGet("ListarMetodologiasNaoAtribuidas")]
        public ActionResult<ListaMetodologiaResult> ListarMetodologiasNaoAtribuidas(string busca, int cursor, int limite)
        {
            var ret = new ListaMetodologiaResult();

            ret = _metodologiaService.ListarMetodologiasNaoAtribuidas(busca, cursor, limite, _usuarioLogado.Cpf);
            return Ok(ret);
        }

        [AllowAnonymous]
        [HttpGet("ListarMetodologia")]
        public ActionResult<ListaMetodologiaComNivelResult> ListarMetodologia(string busca, int cursor, int limite)
        {
            var ret = new ListaMetodologiaComNivelResult();
            ret.Retorno = _metodologiaService.ListarMetodologias(busca, cursor, limite);
            ret.Nivel = _metodologiaService.ListarNivelMetodologia();

            return Ok(ret);
        }

        [AllowAnonymous]
        [HttpGet("ListarNivelMetodologia")]
        public ActionResult<ListaMetodologiaNivelResult> ListarNivelMetodologia()
        {
            var ret = new ApiGenericResult<IEnumerable<NivelDTO>>();
            ret.Retorno = _metodologiaService.ListarNivelMetodologia();

            return Ok(ret);
        }

        [HttpGet("ListarMetodologiasColaborador")]
        public ActionResult<ListaMetodologiaColaboradorResult> ListarMetodologiasColaborador(string cpfColaborador)
        {
            var ret = new ApiGenericResult<IEnumerable<ListaMetodologiaColaboradorResult>>();

            ret.Retorno = _metodologiaService.ListarMetodologiasColaborador(cpfColaborador);

            return Ok(ret);
        }

        [AllowAnonymous]
        [HttpGet("GetMetodologiaById")]
        public ActionResult<MetodologiaDTO> ListarMetodologiasById(int codMetodologia)
        {
            var ret = new ApiGenericResult<MetodologiaDTO>();
            ret.Retorno = _metodologiaService.ListarMetodologiasById(codMetodologia);
            return Ok(ret);
        }

        [AllowAnonymous]
        [HttpPost("AdicionarMetodologiaColaborador")]
        public async Task<ActionResult> AdicionarMetodologiaColaborador([FromBody] List<MetodologiaDTO> param, [FromQuery] bool minhaJornada = false)
        {
            var ret = _metodologiaService.InserirMetodologiaColaborador(param, _usuarioLogado.Cpf, _usuarioLogado.Token, minhaJornada);
            ret.Mensagem = "Metodologia cadastrada com sucesso.";

            return Ok(ret);
        }
        [AllowAnonymous]
        [HttpPatch("AdicionarMetodologia")]
        public async Task<ActionResult> AdicionarMetodologia(List<MetodologiaDTO> param)
        {
            var ret = new ApiGenericResult<IEnumerable<MetodologiaDTO>>();

            _metodologiaService.AdicionarMetodologia(param, _usuarioLogado.Cpf, _usuarioLogado.Token);
            ret.Mensagem = "Metodologia cadastrada com sucesso.";

            return Ok(ret);
        }

        [HttpPatch("AtualizaMetodologiaColaborador")]
        public async Task<ActionResult> AtualizarNivelMetodologiaColaborador([FromBody] MetodologiaDTO param, [FromQuery] bool minhaJornada)
        {
            var ret = new ApiGenericResult<IEnumerable<MetodologiaDTO>>();

            _metodologiaService.AtualizarNivelMetodologiaColaborador(param, _usuarioLogado.Cpf, minhaJornada);

            ret.Mensagem = "Metodologia cadastrada com sucesso.";

            return Ok(ret);
        }

        [HttpDelete("RemoverMetodologiaColaborador")]
        public async Task<ActionResult> RemoverMetodologiaColaborador(int codMetodologia)
        {
            var ret = new ApiGenericResult<IEnumerable<MetodologiaDTO>>();

            _metodologiaService.RemoverMetodologiaColaborador(codMetodologia, _usuarioLogado.Cpf);

            ret.Mensagem = "Metodologia excluida com sucesso.";

            return Ok(ret);
        }

        [HttpPost("GetMetodologiaInfoByDescricao")]
        public async Task<ActionResult> GetMetodologiaInfoByDescricao(GetMetodologiaInfoByDescricao param)
        {
            try
            {
                return Ok(await _metodologiaService
                .GetMetodologiaInfoByDescricao(param.Metodologias));
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message + "\nStackTrace: " + err.StackTrace);
                return StatusCode(500, "Erro interno servidor");
            }
        }
    }
}