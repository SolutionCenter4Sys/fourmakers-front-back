using Colaboracao.Core;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain.Colaborador;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colaborador.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class EstatisticasController : Controller
    {
        private readonly IColaboradorService _colaboradorService;

        public EstatisticasController(IColaboradorService colaboradorService)
        {
            _colaboradorService = colaboradorService;
        }

        [HttpGet("EstatisticasEtnia")]
        public async Task<ActionResult<EstatisticasResult>> EstatisticasEtnia()
        {
            var ret = new EstatisticasResult();
            try
            {
                var retService = await _colaboradorService.EstatisticasEtnia();
                foreach (var item in retService)
                {
                    ret.Estatistica.Add(item);
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("EstatisticasIdade")]
        public async Task<ActionResult<EstatisticasResult>> EstatisticasIdade()
        {
            var ret = new EstatisticasResult();
            try
            {
                var retService = await _colaboradorService.EstatisticasIdade();
                foreach (var item in retService)
                {
                    ret.Estatistica.Add(item);
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("EstatisticasTempoCasa")]
        public async Task<ActionResult<EstatisticasResult>> EstatisticasTempoCasa()
        {
            var ret = new EstatisticasResult();
            try
            {
                var retService = await _colaboradorService.EstatisticasTempoCasa();
                foreach (var item in retService)
                {
                    ret.Estatistica.Add(item);
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("EstatisticasOrientacaoSexual")]
        public async Task<ActionResult<EstatisticasResult>> EstatisticasOrientacaoSexual()
        {
            var ret = new EstatisticasResult();
            try
            {
                var retService = await _colaboradorService.EstatisticasOrientacaoSexual();
                foreach (var item in retService)
                {
                    ret.Estatistica.Add(item);
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("EstatisticasEscolaridade")]
        public async Task<ActionResult<EstatisticasResult>> EstatisticasEscolaridade()
        {
            var ret = new EstatisticasResult();
            try
            {
                var retService = await _colaboradorService.EstatisticasEscolaridade();
                foreach (var item in retService)
                {
                    ret.Estatistica.Add(item);
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("EstatisticasGenero")]
        public async Task<ActionResult<EstatisticasResult>> EstatisticasGenero()
        {
            var ret = new EstatisticasResult();
            try
            {
                var retService = await _colaboradorService.EstatisticasGenero();
                foreach (var item in retService)
                {
                    ret.Estatistica.Add(item);
                }
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("EstatisticasModeloTrabalho")]
        public async Task<ActionResult<List<ModeloTrabalhoDTO>>> EstatisticasModeloTrabalho()
        {
            var ret = new EstatisticasModeloTrabalhoResult();
            try
            {
                var retService = await _colaboradorService.EstatisticasModeloTrabalho();

                return Ok(retService.ModelosTrabalho);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("TotalizadoresPorUnidade")]
        public async Task<ActionResult<TotalizadoresResult>> TotalizadoresPorUnidade()
        {
            var ret = new TotalizadoresResult();
            try
            {
                var retService = await _colaboradorService.TotalizadoresPorUnidade();
                foreach (var item in retService)
                {
                    ret.Totalizadores.Add(item);
                }
                return Ok(ret);
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