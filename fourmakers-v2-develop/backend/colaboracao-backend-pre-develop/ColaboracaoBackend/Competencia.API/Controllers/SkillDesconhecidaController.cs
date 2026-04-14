using Colaboracao.Core.Interfaces;
using Competencia.API.DTOs;
using Competencia.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.SkillDesconhecida;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Competencia.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Competencia/[controller]")]
    [LogAction]
    public class SkillDesconhecidaController : ControllerBase
    {
        private readonly ISkillDesconhecidaService _skillDesconhecidaService;
        private readonly IAspNetUser _aspNetUser;

        public SkillDesconhecidaController(
            ISkillDesconhecidaService skillDesconhecidaService,
            IAspNetUser aspNetUser)
        {
            _skillDesconhecidaService = skillDesconhecidaService;
            _aspNetUser = aspNetUser;
        }

        [HttpGet("ListarSkillDesconhecidasColaborador")]
        public ActionResult<ApiGenericResult<List<SkillDesconhecidaColaboradorDTO>>> ListarSkillDesconhecidasColaborador(string codInternoColaborador)
        {
            var ret = new ApiGenericResult<List<SkillDesconhecidaColaboradorDTO>>();
            try
            {
                if (String.IsNullOrEmpty(codInternoColaborador))
                    codInternoColaborador = _aspNetUser.GetUsuarioLogado().Cpf;
                ret.Retorno = _skillDesconhecidaService.ListarSkillDesconhecidasColaborador(codInternoColaborador).Result;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("GetSkillDesconhecidaInfoByDescricao")]
        public async Task<ActionResult> GetSkillDesconhecidaInfoByDescricao(GetSkillDesconhecidaInfoByDescricao param)
        {
            try
            {
                return Ok(await _skillDesconhecidaService.GetSkillDesconhecidaInfoByDescricao(param.Skills));
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message + "\nStackTrace: " + err.StackTrace);
                return StatusCode(500, "Erro interno servidor");
            }
        }
    }
}