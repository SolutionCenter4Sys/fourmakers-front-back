using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.MapaDeAlocacao.Perfil;
using DataTransferObject.Domain.Match;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using MapaDeAlocacao.Domain.Interfaces.Perfil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace MapaDeAlocacao.API.Controllers.GestaoDeAlocados.Perfil
{
    [Authorize]
    [Route("api/GestaoDeAlocados/[controller]")]
    [HandleException]
    [LogAction]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public PerfilController(IPerfilService gestaoAlocadosService, IAspNetUser aspNetUser)
        {
            _service = gestaoAlocadosService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarPerfis")]
        public async Task<ActionResult> ListarPerfis(string dataInicio, string dataFim, string cliente, int limite, int cursor, string busca, int orgId)
        {
            var clientes = await _service.ListarPerfis(dataInicio, dataFim, cliente, _usuarioLogado.Cpf, limite, cursor, busca, orgId);

            return Ok(clientes);
        }

        [HttpGet("BuscarQuantidadesPessoasMatchSkillSenioridade")]
        public async Task<ActionResult<BuscarQuantidadesPessoasMatchSkillResult>> BuscarQuantidadesPessoasMatchSkill([FromBody] BuscarQuantidadesPessoasMatchSkillInput input)
        {
            var clientes = await _service.BuscarQuantidadesPessoasMatchSkill(input, _usuarioLogado.OrgId);

            return Ok(clientes);
        }

        [HttpGet("ObterEstatisticasMatchSkillSenioridade")]
        public async Task<ActionResult<ObterEstatisticasMatchSkillSenioridadeResult>> ObterEstatisticasMatchSkillSenioridade([FromBody] ObterEstatisticasMatchSkillSenioridadeInput input)
        {
            var clientes = await _service.ObterEstatisticasMatchSkillSenioridade(input);

            return Ok(clientes);
        }

        [HttpPost("ExtrairPerfilDeUmPrompt")]
        public async Task<ActionResult<ExtrairPerfilDeUmPromptResponse>> ExtrairPerfilDeUmPrompt([FromBody] ExtrairPerfilDeUmPromptRequest request)
        {
            var perfil = await _service.ExtrairPerfilDeUmPrompt(request, _usuarioLogado.OrgId, _usuarioLogado.Cpf);

            return Ok(perfil);
        }
    }
}