using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.GestaoPessoa.Pdi;
using GestaoPessoa.API.Models.Pdi;
using GestaoPessoa.Domain.Interfaces.Services.Pdi;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GestaoPessoa.API.Controllers.Pdi
{
    [Authorize]
    [ApiController]
    [Route("api/GestaoPessoa/Pdi/[controller]")]
    [HandleException]
    [LogAction]
    public class PdisDoTimeController : ControllerBase
    {
        private readonly IPdisDoTimeService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public PdisDoTimeController(IPdisDoTimeService service, IAspNetUser aspNetUser)
        {
            _service = service;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();            
        }

        /// <summary>Lista apenas PDIs dos subordinados (toda a hierarquia abaixo do gestor), com paginação. Padrão: página 1, 10 itens.</summary>
        [HttpGet]
        public async Task<ActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
        {
            var result = await _service.ListarPdisDoTimePaginadoAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId, pagina, tamanhoPagina);
            return Ok(result);
        }

        /// <summary>Lista os PDIs de um colaborador (codigo_interno) que pertence ao seu time ou à hierarquia abaixo de qualquer subordinado seu (ex.: time do seu gestor direto).</summary>
        [HttpGet("colaborador/{colaboradorId}")]
        public async Task<ActionResult> ListarPorColaboradorId(string colaboradorId)
        {
            var result = await _service.ListarPdisPorColaboradorIdAsync(colaboradorId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{pdiId}")]
        public async Task<ActionResult> ObterPdiCompleto(Guid pdiId)
        {
            var result = await _service.ObterPdiCompletoDoTimeAsync(pdiId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        // ------ Gestor: criar e gerenciar PDI de um colaborador do time ------

        /// <summary>Gestor: cria PDI para um colaborador do time (colaboradorId = codigo_interno/cpf do colaborador).</summary>
        [HttpPost("colaborador/{colaboradorId}")]
        public async Task<ActionResult> CriarPdiParaColaborador(string colaboradorId, [FromBody] PdiCriarRequestDTO request)
        {
            var result = await _service.CriarPdiParaColaboradorAsync(colaboradorId, _usuarioLogado.Cpf, _usuarioLogado.OrgId, request);
            if (!result.Sucesso)
                return BadRequest(result);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Gestor: atualiza PDI de um colaborador do time.</summary>
        [HttpPut("colaborador/{colaboradorId}/pdi/{pdiId}")]
        public async Task<ActionResult> AtualizarPdiParaColaborador(string colaboradorId, Guid pdiId, [FromBody] PdiAtualizarRequestDTO request)
        {
            var result = await _service.AtualizarPdiParaColaboradorAsync(colaboradorId, pdiId, _usuarioLogado.Cpf, _usuarioLogado.OrgId, request);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Gestor: adiciona action plan ao PDI do colaborador.</summary>
        [HttpPost("colaborador/{colaboradorId}/pdi/{pdiId}/ActionPlans")]
        public async Task<ActionResult> AdicionarActionPlanParaColaborador(string colaboradorId, Guid pdiId, [FromBody] PdiActionPlanInputDTO request)
        {
            var result = await _service.AdicionarActionPlanParaColaboradorAsync(colaboradorId, pdiId, _usuarioLogado.Cpf, _usuarioLogado.OrgId, request);
            if (!result.Sucesso)
                return BadRequest(result);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Gestor: atualiza action plan do PDI do colaborador (description, deadline).</summary>
        [HttpPut("colaborador/{colaboradorId}/pdi/{pdiId}/ActionPlans/{actionPlanId}")]
        public async Task<ActionResult> AtualizarActionPlanParaColaborador(string colaboradorId, Guid pdiId, Guid actionPlanId, [FromBody] PdiActionPlanInputDTO request)
        {
            var result = await _service.AtualizarActionPlanParaColaboradorAsync(colaboradorId, pdiId, actionPlanId, _usuarioLogado.Cpf, _usuarioLogado.OrgId, request);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Gestor: remove action plan do PDI do colaborador.</summary>
        [HttpDelete("colaborador/{colaboradorId}/pdi/{pdiId}/ActionPlans/{actionPlanId}")]
        public async Task<ActionResult> RemoverActionPlanParaColaborador(string colaboradorId, Guid pdiId, Guid actionPlanId)
        {
            var result = await _service.RemoverActionPlanParaColaboradorAsync(colaboradorId, pdiId, actionPlanId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Gestor: marca action plan como concluído.</summary>
        [HttpPatch("colaborador/{colaboradorId}/pdi/{pdiId}/ActionPlans/{actionPlanId}/Complete")]
        public async Task<ActionResult> ConcluirActionPlanParaColaborador(string colaboradorId, Guid pdiId, Guid actionPlanId)
        {
            var result = await _service.ConcluirActionPlanParaColaboradorAsync(colaboradorId, pdiId, actionPlanId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Gestor: aprova o PDI (criado pelo gestor). Colaborador criou os planos de ação; ao aprovar, PDI vai para Andamento (IN_PROGRESS).</summary>
        [HttpPatch("colaborador/{colaboradorId}/pdi/{pdiId}/aprovar")]
        public async Task<ActionResult> AprovarPdiParaColaborador(string colaboradorId, Guid pdiId)
        {
            var result = await _service.AprovarPdiParaColaboradorAsync(colaboradorId, pdiId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Gestor: envia evidência para o PDI do colaborador (multipart/form-data: arquivo e link opcionais; ao menos um obrigatório).</summary>
        [HttpPost("colaborador/{colaboradorId}/pdi/{pdiId}/evidencias")]
        [Consumes("multipart/form-data", "application/x-www-form-urlencoded")]
        public async Task<ActionResult> UploadEvidenciaParaColaborador(string colaboradorId, Guid pdiId)
        {
            var (arquivo, link, tipo) = await PdiEvidenciaMultipartReader.ReadAsync(Request);
            byte[] bytes = null;
            var docName = arquivo?.FileName;
            var docMime = arquivo?.ContentType;
            long? docSize = arquivo?.Length;
            if (arquivo != null && arquivo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await arquivo.CopyToAsync(ms);
                    bytes = ms.ToArray();
                }
            }
            var result = await _service.UploadEvidenciaParaColaboradorAsync(colaboradorId, pdiId, _usuarioLogado.Cpf, _usuarioLogado.OrgId, docName, null, docMime, docSize, tipo ?? "CERTIFICADO", bytes, link);
            if (!result.Sucesso)
                return BadRequest(result);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Gestor: lista evidências do PDI do colaborador.</summary>
        [HttpGet("colaborador/{colaboradorId}/pdi/{pdiId}/evidencias")]
        public async Task<ActionResult> ListarEvidenciasParaColaborador(string colaboradorId, Guid pdiId)
        {
            var result = await _service.ListarEvidenciasParaColaboradorAsync(colaboradorId, pdiId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Gestor: obtém a evidência (arquivo para download ou JSON com link se for só URL externa).</summary>
        [HttpGet("colaborador/{colaboradorId}/pdi/{pdiId}/evidencias/{evidenciaId}")]
        public async Task<ActionResult> ObterEvidenciaParaColaborador(string colaboradorId, Guid pdiId, Guid evidenciaId)
        {
            var result = await _service.ObterEvidenciaParaColaboradorAsync(colaboradorId, pdiId, evidenciaId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            if (!result.Sucesso)
                return BadRequest(result);
            var r = result.Retorno;
            if (r.Content != null && r.Content.Length > 0)
                return File(r.Content, r.ContentType ?? "application/octet-stream", r.FileName);
            return Ok(result);
        }
    }
}
