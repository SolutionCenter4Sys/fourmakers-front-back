using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.GestaoPessoa.Pdi;
using DataTransferObject.Domain.Usuario;
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
    public class MeusPdisController : ControllerBase
    {
        private readonly IMeusPdisService _service;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public MeusPdisController(IMeusPdisService service, IAspNetUser aspNetUser)
        {
            _service = service;
           _usuarioLogado = aspNetUser.GetUsuarioLogado();                        
        }

        [HttpGet]
        public async Task<ActionResult> Listar()
        {
            var result = await _service.ListarMeusPdisAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Criar([FromBody] PdiCriarRequestDTO request)
        {
            var result = await _service.CriarPdiAsync(_usuarioLogado.Cpf, _usuarioLogado.OrgId, request);
            if (!result.Sucesso)
                return BadRequest(result);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Atualizar(Guid id, [FromBody] PdiAtualizarRequestDTO request)
        {
            var result = await _service.AtualizarPdiAsync(id, _usuarioLogado.Cpf, _usuarioLogado.OrgId, request);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("{pdiId}/action-plans")]
        public async Task<ActionResult> AdicionarActionPlan(Guid pdiId, [FromBody] PdiActionPlanInputDTO request)
        {
            var result = await _service.AdicionarActionPlanAsync(pdiId, _usuarioLogado.Cpf, _usuarioLogado.OrgId, request);
            if (!result.Sucesso)
                return BadRequest(result);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPatch("{pdiId}/action-plans/{actionPlanId}/complete")]
        public async Task<ActionResult> ConcluirActionPlan(Guid pdiId, Guid actionPlanId)
        {
            var result = await _service.ConcluirActionPlanAsync(pdiId, actionPlanId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            if (!result.Sucesso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("{pdiId}/evidencias")]
        [Consumes("multipart/form-data", "application/x-www-form-urlencoded")]
        public async Task<ActionResult> UploadEvidencia(Guid pdiId)
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
            var result = await _service.UploadEvidenciaAsync(pdiId, _usuarioLogado.Cpf, _usuarioLogado.OrgId, docName, null, docMime, docSize, tipo ?? "CERTIFICADO", bytes, link);
            if (!result.Sucesso)
                return BadRequest(result);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpGet("{pdiId}/evidencias")]
        public async Task<ActionResult> ListarEvidencias(Guid pdiId)
        {
            var result = await _service.ListarEvidenciasAsync(pdiId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        /// <summary>Obtém a evidência: se houver arquivo, retorna o binário para download; se for só URL, retorna 200 JSON (<c>retorno.link</c>).</summary>
        [HttpGet("{pdiId}/evidencias/{evidenciaId}")]
        public async Task<ActionResult> ObterEvidencia(Guid pdiId, Guid evidenciaId)
        {
            var result = await _service.ObterEvidenciaAsync(pdiId, evidenciaId, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            if (!result.Sucesso)
                return BadRequest(result);
            var r = result.Retorno;
            if (r.Content != null && r.Content.Length > 0)
                return File(r.Content, r.ContentType ?? "application/octet-stream", r.FileName);
            return Ok(result);
        }
    }
}
