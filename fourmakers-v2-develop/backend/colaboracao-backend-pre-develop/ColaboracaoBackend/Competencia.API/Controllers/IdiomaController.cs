using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Competencia.API.DTOs;
using Competencia.Domain.Impl.Services;
using Competencia.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Idioma;
using DataTransferObject.Domain.Nivel;
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
    public class IdiomaController : ControllerBase
    {
        private IIdiomaService _idiomaService;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;
        private readonly IAspNetUser _aspNetUser;

        public IdiomaController(IIdiomaService idiomaService, IVerificaSeCpfESistemico verificaCpfSistemico, IAspNetUser aspNetUser)
        {
            _idiomaService = idiomaService;
            _verificaCpfSistemico = verificaCpfSistemico;
            _aspNetUser = aspNetUser;
        }

        [AllowAnonymous]
        [HttpGet("ListarIdioma")]
        public ActionResult<ListaIdiomaResult> ListarIdioma(string busca, int cursor, int limite)
        {
            var ret = new ListaIdiomaResult();
            var retService = _idiomaService.ListarIdioma(busca, cursor, limite);

            ret.Idioma = retService
            .Select(x => new ItemPerfilDTO
            {
                Descricao = x.Descricao,
                Id = x.Id,
                Pendente = x.Pendente
            }).ToList();

            ret.Nivel = _idiomaService.ListaNivelIdioma();
            return Ok(ret);
        }

        [HttpGet("GetGroupIdiomaById")]
        public async Task<ActionResult<List<CompetenciaGroupDTO>>> GetGroupIdiomaById(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return BadRequest("Informe ao menos um id.");

            // separa por vírgula → converte para long
            var listaIds = ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.Parse(x.Trim()))
                .ToList();

            var competencias = await _idiomaService.GetGroupIdiomaByIds(listaIds);
            return Ok(competencias);
        }


        [HttpGet("GetIdiomaById")]
        public ActionResult<IdiomaResult> GetIdiomaById(int id)
        {
            var ret = new IdiomaResult();
            var retService = _idiomaService.GetIdiomaById(id);
            if (retService != null)
            {
                ret.Idioma = retService;
            }
            else
            {
                ret.Idioma = new IdiomaDTO
                {
                    Descricao = null,
                    Id = id
                };
                ret.Sucesso = false;
            }

            return Ok(ret);
        }

        [HttpPost("AdicionarIdioma")]
        public ActionResult<IdiomaResult> AdicionarIdioma(AddIdiomaParam param)
        {
            var ret = new IdiomaResult();
            var retService = _idiomaService.AdicionarIdioma(param.Descricao);
            ret.Idioma = retService;
            return Ok(ret);
        }

        [HttpGet("ListarIdiomaColaborador")]
        public ActionResult ListarIdiomaColaborador(string cpfColaborador)
        {
            var ret = new ListaIdiomaColaboradorResult();
            var retService = _idiomaService.ListarIdiomaColaborador(cpfColaborador);
            ret.Idioma = retService;

            return Ok(ret);
        }

        [HttpPost("AdicionarIdiomaColaborador")]
        public ActionResult<IdiomaColaboradorResult> AdicionarIdiomaColaborador([FromBody] List<AddIdiomaColabParam> param, [FromQuery] bool minhaJornada = false)
        {
            var ret = new IdiomaColaboradorResult();
            return _idiomaService.AdicionarIdiomaColaboradorEmLote(param, _aspNetUser.GetUsuarioLogado().Cpf, minhaJornada);
        }

        [HttpPost("RemoverIdiomaColaborador")]
        public ActionResult<StatusResult> RemoverIdiomaColaborador(RemoveIdiomaColabParam param)
        {
            var ret = new StatusResult();
            var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(_aspNetUser.GetUsuarioLogado().Cpf);
            _idiomaService.RemoveIdiomaColaborador(param.IdiomaId, cpfRequest);
            ret.Sucesso = true;
            return Ok(ret);
        }

        [HttpGet("ListarNivelIdioma")]
        public ActionResult<ListaNivelResult> ListarNivelIdioma()
        {
            var ret = new ListaNivelResult();
            var retService = _idiomaService.ListaNivelIdioma();
            retService.ForEach(x => ret.Niveis.Add(x));
            return Ok(ret);
        }

        [HttpPost("AtualizaIdiomaColaborador")]
        public IdiomaResult AtualizaIdiomaColaborador([FromBody] AdicionarRemoverIdiomaParam param, [FromQuery] bool minhaJornada = false)
        {
            var ret = new IdiomaResult();
            var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(_aspNetUser.GetUsuarioLogado().Cpf);
            try
            {
                var retService = _idiomaService.AlterarIdiomaColaborador(cpfRequest, param.Id, param.NivelId, param.GestorExternoPerfil, minhaJornada);

                ret.IdiomaId = retService.Idioma.Id;
            }
            catch (Exception e)
            {
                ret = new IdiomaResult
                {
                    Sucesso = false,
                    IdiomaId = param.Id,
                    Mensagem = e.Message
                };
            }
            ret.Sucesso = true;
            return ret;
        }

        [HttpGet("ListarIdiomasNaoAtribuidos")]
        public ActionResult<ListaIdiomaResult> ListarIdiomasNaoAtribuidos(string busca, int cursor, int limite)
        {
            var ret = _idiomaService.ListarIdiomasNaoAtribuidos(busca, cursor, limite, _aspNetUser.GetUsuarioLogado().Cpf);
            return Ok(ret);
        }

        [HttpPost("GetIdiomaInfoByDescricao")]
        public async Task<ActionResult> GetIdiomaInfoByDescricao(GetIdiomaInfoByDescricao param)
        {
            try
            {
                return Ok(_idiomaService
                .GetIdiomaInfoByDescricao(param.Idiomas));
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message + "\nStackTrace: " + err.StackTrace);
                return StatusCode(500, "Erro interno servidor");
            }
        }
    }
}