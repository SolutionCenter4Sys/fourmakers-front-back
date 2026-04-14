using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Financeiro.Rubrica.RubricaColaborador;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.Rubrica.RubricaColaborador;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using DataTransferObject.Domain.Apontamento;
using DataTransferObject.Domain.Base;

using Logs.Infra.Attributes;

namespace Financeiro.API.Controllers.Rubrica
{
    [Authorize]
    [Route("api/Financeiro/Rubrica/Colaborador")]
    [HandleException]
    [ApiController]
    [LogAction]
    public class RubricaColaboradorController : ControllerBase
    {
        private readonly IRubricaColaboradorService _rubricaColaboradorService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public RubricaColaboradorController(IRubricaColaboradorService rubricaColaboradorService, IAspNetUser aspNetUser)
        {
            _rubricaColaboradorService = rubricaColaboradorService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarRubricasColaborador")]
        public async Task<ActionResult> ListarRubricasColaborador([FromQuery] string? unidadeId, string? codigoInternoColaborador, int? mes, int? ano, string? rubricaId, int cursor, int limite)
        {
            var perfis = await _rubricaColaboradorService.ListarRubricasColaborador(_usuarioLogado.Cpf, _usuarioLogado.OrgId, unidadeId, codigoInternoColaborador, mes, ano, rubricaId, cursor, limite);
            return Ok(perfis);
        }

        [HttpGet("ObterRubricaColaboradorPorId")]
        public async Task<ActionResult> ObterRubricaColaboradorPorId(Guid id)
        {
            var perfil = await _rubricaColaboradorService.ObterRubricaColaboradorPorId(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(perfil);
        }

        [HttpPost("InserirRubricaColaborador")]
        public async Task<ActionResult> InserirRubricaColaborador([FromBody] RubricaColaboradorInput param)
        {
            var result = await _rubricaColaboradorService.InserirRubricaColaborador(param, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPut("AtualizarRubricaColaborador")]
        public async Task<ActionResult> AtualizarRubricaColaborador(Guid id, [FromBody] RubricaColaboradorInput param)
        {
            var result = await _rubricaColaboradorService.AtualizarRubricaColaborador(param, id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpDelete("DeletarRubricaColaborador")]
        public async Task<ActionResult> DeletarRubricaColaborador(Guid id)
        {
            var result = await _rubricaColaboradorService.DeletarRubricaColaborador(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId);

            if (!result.Sucesso)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
        
        [HttpGet("ListarMesEAnosLancadosPorOrgId")]
        public async Task<ActionResult<ApiGenericResult<List<VigenciaDTO>>>> ListarMesEAnosLancadosPorOrgId()
        {
            var vigencias = await _rubricaColaboradorService.ListarMesEAnosLancadosPorOrgId( _usuarioLogado.OrgId);
            return Ok(vigencias);
        }

    }
}
