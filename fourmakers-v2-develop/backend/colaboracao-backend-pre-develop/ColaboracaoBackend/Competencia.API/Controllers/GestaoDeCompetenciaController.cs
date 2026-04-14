using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Competencia.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Competencia.GestaoDeCompetencia;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace Competencia.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class GestaoDeCompetenciaController : ControllerBase
    {
        private readonly IGestaoDeCompetenciaService _gestaoDeHabilidadesService;
        private readonly UsuarioLogadoDTO _usuarioLogadoDTO;

        public GestaoDeCompetenciaController(IGestaoDeCompetenciaService gestaoDeHabilidadesService, IAspNetUser aspnetUser)
        {
            _gestaoDeHabilidadesService = gestaoDeHabilidadesService;
            _usuarioLogadoDTO = aspnetUser.GetUsuarioLogado();
        }

        [HttpPost("AlterarCategoriaDaHabilidade")]
        public async Task<ActionResult<ApiGenericResult<AdicionarCompetenciaDTO>>> AlterarCategoriaDaHabilidade([FromBody] AlterarCategoriaHabilidadeParam input)
        {
            var ret = new ApiGenericResult<AdicionarCompetenciaDTO>();
            try
            {
                var retService = await _gestaoDeHabilidadesService.AlterarCategoriaDaHabilidade(input.Id, input.CategoriaAtual, input.CategoriaDestino, _usuarioLogadoDTO.Cpf, _usuarioLogadoDTO.Token, _usuarioLogadoDTO.OrgId);
                ret.Retorno = retService;
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