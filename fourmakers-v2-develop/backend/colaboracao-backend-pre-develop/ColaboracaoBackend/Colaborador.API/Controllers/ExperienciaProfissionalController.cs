using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Experiencia;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Colaborador.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class ExperienciaProfissionalController : Controller

    {
        private readonly IExperienciaProfissionalService _experienciaProfissionalService;
        private readonly ITokens _tokens;
        private readonly IAspNetUser _aspNetUser;
        private readonly ILogCore _log;
        private readonly IConfiguration _configuration;
        public ExperienciaProfissionalController(IExperienciaProfissionalService experienciaProfissional, ITokens tokens, IAspNetUser aspNetUser, ILogCore log, IConfiguration configuration)
        {
            _experienciaProfissionalService = experienciaProfissional;
            _tokens = tokens;
            _aspNetUser = aspNetUser;
            _log = log;
            _configuration = configuration;
        }

        [HttpPost("AdicionarExperienciaProfissional")]
        public ActionResult<ExperienciaResult> AdicionarExperienciaProfissional(AddExperienciaDTO param)
        {
            var ret = new ExperienciaResult();
            try
            {
                ret.experiencia = _experienciaProfissionalService.AdicionarNovaExperiencia(param.Atividades, _aspNetUser.GetUsuarioLogado().Cpf, param.Funcao, param.Empresa, param.DataInicio, param.DataSaida, param.Projetos);
                return Ok(ret);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarExperienciaProfissionalPorCpf")]
        public ActionResult<ListaExperienciaProfissionalResult> ListarExperienciaProfissional(string busca, int cursor, int limite)
        {
            var ret = new ListaExperienciaProfissionalResult();
            try
            {
                ret.ExperienciaEmpresas = _experienciaProfissionalService.ListarExperienciaProfissionalAgrupada(busca, cursor, limite, _aspNetUser.GetUsuarioLogado().Cpf, _aspNetUser.GetUsuarioLogado().OrgId);

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("GetExperienciaProfissionalById")]
        public ActionResult<ExperienciaResult> GetExperienciaProfissionalById(long id)
        {
            var ret = new ExperienciaResult();
            try
            {
                ret.experiencia = _experienciaProfissionalService.GetExperinciaProfissionalById(id);

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("RemoverExperienciaProfissional")]
        public ActionResult<StatusResult> RemoverExperienciaProfissional(RemoverExperienciaProfissionalDTO param)
        {
            var ret = new StatusResult();
            try
            {
                _experienciaProfissionalService.RemoverExperienciaProfissional(param.ExperienciaId, _aspNetUser.GetUsuarioLogado().Cpf);
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (KeyNotFoundException e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AtualizaExperienciaProfissional")]
        public ActionResult<UpdateExperienciaResult> AtualizaExperienciaProfissional(UpdateExperienciaDTO param)
        {
            var ret = new UpdateExperienciaResult();
            try
            {
                ret.experiencia = _experienciaProfissionalService
                    .AtualizarExperiencia(param.Id, param.Atividades, _aspNetUser.GetUsuarioLogado().Cpf, param.Funcao, param.Empresa, param.DataInicio, param.DataSaida, param.Projetos);
                return Ok(ret);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AutoCompleteSugestaoEmpresa")]
        public ActionResult<SugestaoEmpresaResult> AutoCompleteSugestaoEmpresa(String nomeEmpresa, int limite, int cursor)
        {
            var ret = new SugestaoEmpresaResult();
            try
            {
                ret.sugestaoEmpresas = _experienciaProfissionalService
                    .AutoCompleteEmpresa(nomeEmpresa, limite, cursor);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AutoCompleteSugestaoProjeto")]
        public ActionResult<SugestaoProjetoResult> AutoCompleteSugestaoProjeto(String nomeProjeto, int limite, int cursor)
        {
            var ret = new SugestaoProjetoResult();
            try
            {
                ret.sugestaoProjetos = _experienciaProfissionalService
                    .AutoCompleteProjeto(nomeProjeto, limite, cursor);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }
        [HttpPost("AdicionarSobreColaborador")]
        public ActionResult<StatusResult> AdicionarTextoSobre(string sobre)
        {
            var ret = new StatusResult();
            try
            {
                _experienciaProfissionalService
                    .AdicionarSobre(_aspNetUser.GetUsuarioLogado().Cpf, sobre);
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }
        [HttpPost("RemoverSobreColaborador")]
        public ActionResult<StatusResult> RemoverSobreColaborador()
        {
            var ret = new StatusResult();
            try
            {
                _experienciaProfissionalService
                    .RemoverSobre(_aspNetUser.GetUsuarioLogado().Cpf);
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("BuscarSobreColaborador")]
        public ActionResult<ColaboradorSobreResult> BuscarSobreColaborador()
        {
            var ret = new ColaboradorSobreResult();
            try
            {
                var sobre = _experienciaProfissionalService.BuscarSobre(_aspNetUser.GetUsuarioLogado().Cpf);
                ret.ColaboradorSobre = sobre ?? new ColaboradorSobreDTO() { Descricao = "" };
                if (sobre == null)
                {
                    return Ok(ret);
                }
                return Ok(ret);
            }
            catch (ArgumentException e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return BadRequest(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }
    }
}