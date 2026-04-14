using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Projeto.ProjetoOrg;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projeto.Domain.Interfaces.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Projeto.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/Projeto/[controller]")]
    [ApiController]
    [LogAction]
    public class ProjetoOrgController : Controller
    {
        private readonly IProjetoOrgService _projetoOrgService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public ProjetoOrgController(IAspNetUser aspNetUser, IProjetoOrgService projetoOrgService)
        {
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _projetoOrgService = projetoOrgService;
        }

        [HttpPost("CadastrarProjeto")]
        public async Task<ActionResult> CadastrarProjeto(ProjetoOrgDTO param)
        {
            var ret = new ApiGenericResult<ProjetoOrgDetalhesDTO>();

            ret.Retorno = await _projetoOrgService.CadastrarProjeto(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (ret.Sucesso)
            {
                ret.Mensagem = "Projeto cadastrado com sucesso.";
                return Ok(ret);
            }
            else
            {
                throw new Exception("Falha ao cadastrar o projeto.");
            }
        }

        [HttpPatch("EditarProjeto")]
        public async Task<ActionResult> EditarProjeto(ProjetoOrgDTO param)
        {
            var ret = new ApiGenericResult<ProjetoOrgDetalhesDTO>();

            ret.Retorno = await _projetoOrgService.EditarProjeto(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            ret.Mensagem = "Projeto editado com sucesso.";

            return Ok(ret);
        }

        [HttpGet("ListarProjetos")]
        public async Task<ActionResult<IEnumerable<ProjetosOrgResult>>> ListarProjetos(int cursor, int limite, int codStatus, string nomeProjeto)
        {
            var ret = new ApiGenericResult<IEnumerable<ProjetosOrgResult>>();

            ret.Retorno = await _projetoOrgService.ListarProjetos(_usuarioLogado.OrgId, cursor, limite, codStatus, nomeProjeto);
            return Ok(ret);
        }

        [HttpGet("ObterProjetoPorCodigo")]
        public async Task<ActionResult> ObterProjetoPorCodigo(string codProjeto)
        {
            var ret = new ApiGenericResult<ProjetoOrgDetalhesDTO>();

            ret.Retorno = await _projetoOrgService.ObterProjetoPorCodigo(codProjeto, _usuarioLogado.OrgId);
            ret.Mensagem = "Consulta efetuada com sucesso!";

            return Ok(ret);
        }

        [HttpGet("ListarGestoresProjeto")]
        public async Task<ActionResult> ListarGestoresProjeto(string codigoDiretoria, string codigoDepartamento, string codigoGestorAdm)
        {
            var ret = new ApiGenericResult<List<ProjetoOrgColaboradorGerenteDetalhesDTO>>();
            ret.Retorno = await _projetoOrgService.ListarGestoresProjeto(codigoDiretoria, codigoDepartamento, codigoGestorAdm, _usuarioLogado.OrgId, _usuarioLogado.Cpf);
            ret.Mensagem = "Consulta efetuada com sucesso!";
            return Ok(ret);
        }
        
        [HttpGet("ListarProjetosDoCliente")]
        public async Task<ActionResult<IEnumerable<ProjetoSimplesDTO>>> ListarProjetosDoCliente(string codigoCliente)
        {
            var ret = new ApiGenericResult<IEnumerable<ProjetoSimplesDTO>>();
            ret.Retorno = await _projetoOrgService.ListarProjetosDoCliente(codigoCliente, _usuarioLogado.OrgId);
            ret.Mensagem = "Consulta efetuada com sucesso!";
            return Ok(ret);
        }
    }
}