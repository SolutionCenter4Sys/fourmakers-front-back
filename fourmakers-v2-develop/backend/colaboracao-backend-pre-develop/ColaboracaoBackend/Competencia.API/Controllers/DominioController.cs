using Colaboracao.Core;
using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Competencia.API.DTOs;
using Competencia.Domain.Impl.Services;
using Competencia.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Dominio;
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
    [Route("api/Competencia/[controller]")]
    [ApiController]
    [HandleException]
    [LogAction]
    public class DominioController : ControllerBase
    {
        private IDominioService _dominioService;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public DominioController(IDominioService dominioService, IVerificaSeCpfESistemico verificaCpfSistemico, IAspNetUser aspNetUser)
        {
            _dominioService = dominioService;
            _verificaCpfSistemico = verificaCpfSistemico;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [AllowAnonymous]
        [HttpGet("ListarDominio")]
        public ActionResult<ListaDominioResult> ListarDominio(string busca, int cursor, int limite)
        {
            var ret = new ListaDominioResult();
            ret.Dominio = _dominioService.ListDominio(busca, cursor, limite);
            ret.Nivel = _dominioService.ListaNivelDominio();
            return Ok(ret);
        }

        [HttpGet("GetGroupDominioById")]
        public async Task<ActionResult<List<CompetenciaGroupDTO>>> GetGroupDominioById(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return BadRequest("Informe ao menos um id.");

            // separa por vírgula → converte para long
            var listaIds = ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.Parse(x.Trim()))
                .ToList();

            var competencias = await _dominioService.GetGroupDominioByIds(listaIds);
            return Ok(competencias);
        }

        [HttpGet("GetDominioById")]
        public ActionResult<DominioResult> GetDominioById(long id)
        {
            var ret = new DominioResult();
            ret.Dominio = _dominioService.GetDominioById(id);
            return Ok(ret);
        }

        [HttpPost("AdicionarDominio")]
        public ActionResult<DominioResult> AdicionarDominio(AddDominioParam param)
        {
            var ret = new DominioResult();
            var retDominio = _dominioService.AddDominio(param.Descricao);
            ret.Dominio = new ItemPerfilDTO()
            {
                Descricao = retDominio.Descricao,
                Id = retDominio.Id
            };
            return Ok(ret);
        }

        [HttpGet("ListarDominioColaborador")]
        public ActionResult ListarDominioColaborador(string cpfColaborador)
        {
            var ret = new ListaDominioColaboradorResult();
            var retService = _dominioService.ListDominioColaborador(cpfColaborador);
            foreach (var item in retService)
                ret.Dominio.Add(item);
            return Ok(ret);
        }

        [HttpPost("AdicionarDominioColaborador")]
        public ActionResult AdicionarDominioColaborador([FromBody] List<AddDominioColabParam> param, [FromQuery] bool minhaJornada = false)
        {
            var ret = new DominioColaboradorResult();
            ret = _dominioService.AdicionarDominioColaboradorEmLote(param, minhaJornada, _usuarioLogado.Cpf);
            return Ok(ret);
        }

        [HttpPost("RemoverDominioColaborador")]
        public ActionResult<StatusResult> RemoverDominioColaborador(RemoveDominioColabParam param)
        {
            var ret = new StatusResult();
            var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);
            ret.Sucesso = _dominioService.RemoveDominioColaborador(param.DominioId, cpfRequest);
            return Ok(ret);
        }

        [HttpGet("ListarNivelDominio")]
        public ActionResult<ListaNivelResult> ListarNivelDominio()
        {
            var ret = new ListaNivelResult();
            ret.Niveis = _dominioService.ListaNivelDominio();
            return Ok(ret);
        }

        [HttpPost("AtualizaDominioColaborador")]
        public ItemPerfilResult AtualizaDominioColaborador([FromBody] AdicionarRemoverItemParam param, [FromQuery] bool minhaJornada)
        {
            var ret = new ItemPerfilResult();
            try
            {
                var retService = _dominioService.AlterarDominioColaborador(param.Cpf, param.Id, param.NivelId, param.GestorExternoPerfil, minhaJornada, _usuarioLogado.Cpf);
                ret.ItemId = retService.Id;
            }
            catch (Exception e)
            {
                ret = new ItemPerfilResult
                {
                    Sucesso = false,
                    ItemId = param.Id,
                    Mensagem = e.Message
                };
            }
            ret.Sucesso = true;
            return ret;
        }

        [AllowAnonymous]
        [HttpGet("ListarDominiosNaoAtribuidos")]
        public ActionResult<ListaDominioResult> ListarDominiosNaoAtribuidos(string busca, int cursor, int limite)
        {
            var ret = new ListaDominioResult();
            ret = _dominioService.ListarDominiosNaoAtribuidos(busca, cursor, limite);
            ret.Sucesso = true;
            return Ok(ret);
        }

        [HttpPost("GetDominioInfoByDescricao")]
        public async Task<ActionResult> GetDominioInfoByDescricao(GetDominioInfoByDescricao param)
        {
            try
            {
                return Ok(await _dominioService
                .GetDominioInfoByDescricao(param.Dominios));
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message + "\nStackTrace: " + err.StackTrace);
                return StatusCode(500, "Erro interno servidor");
            }
        }
    }
}