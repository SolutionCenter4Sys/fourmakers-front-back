using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Dependentes;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Colaborador.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class DependenteController : ControllerBase
    {
        private readonly IColaboradorService _colaboradorService;
        private readonly IVerificaSeCpfESistemico _verificaSeCpfESistemico;
        private readonly IAspNetUser _aspNetUser;

        public DependenteController(IColaboradorService colaboradorService, IVerificaSeCpfESistemico verificaSeCpfESistemico, IAspNetUser aspNetUser)
        {
            _colaboradorService = colaboradorService;
            _verificaSeCpfESistemico = verificaSeCpfESistemico;
            _aspNetUser = aspNetUser;
        }

        [HttpPost("AlterarDadosDependentes")]
        public ActionResult<AlterarDependentesResult> AlterarDadosDependentes(AlterarDependentesDTO param)
        {
            var ret = new AlterarDependentesResult();
            try
            {
                var retDependentes = _colaboradorService.AlterarDadosDependenteColaborador(param, _aspNetUser.GetUsuarioLogado().Cpf);
                ret.AlterarDependentesDTO.Id = retDependentes.Id;
                ret.AlterarDependentesDTO.NomeCompleto = retDependentes.NomeCompleto;
                ret.AlterarDependentesDTO.DataNascimento = retDependentes.DataNascimento;
                ret.AlterarDependentesDTO.Rg = retDependentes.Rg;
                ret.AlterarDependentesDTO.Cpf = retDependentes.Cpf;
                ret.AlterarDependentesDTO.PortadorDeficiencia = retDependentes.PortadorDeficiencia;
                ret.AlterarDependentesDTO.RequerAjudaQual = retDependentes.RequerAjudaQual;
                ret.AlterarDependentesDTO.Ativo = 1;
                ret.AlterarDependentesDTO.TipoDependente = retDependentes.TipoDependente;

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AdicionarDadosDependentes")]
        public ActionResult<AdicionarDependentesDTO> AdicionarDadosDependentes(AdicionarDependentesDTO param)
        {
            var ret = new AdicionarDependentesDTO();
            var cpf = _aspNetUser.GetUsuarioLogado().Cpf;
            try
            {
                var retDependentes = _colaboradorService.AdicionarDadosDependenteColaborador(param, cpf);
                ret.NomeCompleto = retDependentes.NomeCompleto;
                ret.DataNascimento = retDependentes.DataNascimento;
                ret.Rg = retDependentes.Rg;
                ret.Cpf = retDependentes.Cpf;
                ret.PortadorDeficiencia = retDependentes.PortadorDeficiencia;
                ret.RequerAjudaQual = retDependentes.RequerAjudaQual;
                ret.TipoDependenteId = retDependentes.tipoDependenteId;

                return Ok(ret);
            }
            catch (Exception e)
            {
                return StatusCode(500, ret);
            }
        }

        [HttpPost("RemoverDadosDependentes")]
        public ActionResult<StatusResult> RemoverDadosDependentes(RemoverDependentesDTO param)
        {
            var ret = new StatusResult();
            try
            {
                _colaboradorService.RemoveDependenteColaborador(param.Id, _aspNetUser.GetUsuarioLogado().Cpf);
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

        [HttpGet("ListarDependenteColaborador")]
        public DependentesResult ListarDependenteColaborador()
        {
            var ret = new DependentesResult();
            try
            {
                var retService = _colaboradorService.ListarDadosColaboradorDependente(_aspNetUser.GetUsuarioLogado().Cpf);

                foreach (var item in retService)
                {
                    ret.DependentesDTO.Add(item);
                }
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("ListarTipoDependente")]
        public List<TipoDependenteDTO> ListarTipoDependente()
        {
            var ret = _colaboradorService.ListarTipoDependente();
            return ret;
        }
    }
}
