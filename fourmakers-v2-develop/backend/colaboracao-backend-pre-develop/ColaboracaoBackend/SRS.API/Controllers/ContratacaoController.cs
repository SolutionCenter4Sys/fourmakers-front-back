using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Contratacao;
using DataTransferObject.Domain.Candidatura;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SRS.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Colaborador.API.DTOs;

namespace SRS.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/Vaga/[controller]")]
    [ApiController]
    [LogAction]
    public class ContratacaoController : ControllerBase
    {
        private readonly UsuarioLogadoDTO _usuarioLogadoDTO;
        private readonly ILogCore _log;
        private readonly IAspNetUser _aspNetUser;
        private readonly IContratacaoService _contratacaoService;
        private readonly ITemplateContratacaoLogService _logService;
        private readonly ISistemasLiberadosService _sistemasLiberadosService;
        private readonly IDiretoriosService _diretoriosService;
        private readonly IGruposEmailsService _gruposEmailsService;

        public ContratacaoController(IAspNetUser aspNetUser, ILogCore log, IContratacaoService contratacaoService, ITemplateContratacaoLogService logService, ISistemasLiberadosService sistemasLiberadosService, IDiretoriosService diretoriosService, IGruposEmailsService gruposEmailsService)
        {
            _aspNetUser = aspNetUser;
            _usuarioLogadoDTO = aspNetUser.GetUsuarioLogado();
            _log = log;
            _contratacaoService = contratacaoService;
            _logService = logService;
            _sistemasLiberadosService = sistemasLiberadosService;
            _diretoriosService = diretoriosService;
            _gruposEmailsService = gruposEmailsService;
        }

        [HttpPost("CriarTemplate")]
        public async Task<ActionResult<ApiGenericResult<TemplateDTO>>> CriarTemplate([FromBody] TemplateDTO template)
        {
            try
            {
                var result = await _contratacaoService.CriarTemplateAsync(template, _aspNetUser.GetUsuarioLogado().Cpf);
                
                if (!result.Sucesso)
                {
                    return BadRequest(result);
                }
                
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                // Exceções de validação - retornar BadRequest
                return BadRequest(new ApiGenericResult<TemplateDTO>
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                });
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<TemplateDTO> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ObterTemplate/{id}")]
        public async Task<ActionResult<ApiGenericResult<TemplateDTO>>> ObterTemplate(Guid id)
        {
            try
            {
                var result = await _contratacaoService.ObterTemplatePorIdAsync(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<TemplateDTO> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ObterTemplatePorCandidatura/{tbCandidatoVagaId}")]
        public async Task<ActionResult<ApiGenericResult<TemplateDTO>>> ObterTemplatePorCandidatura(Guid tbCandidatoVagaId)
        {
            try
            {
                var result = await _contratacaoService.ObterTemplatePorCandidatoVagaIdAsync(tbCandidatoVagaId);
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<TemplateDTO> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ListarTemplates")]
        public async Task<ActionResult<ApiGenericResult<List<TemplateDTO>>>> ListarTemplates(int limite = 10, int cursor = 0)
        {
            try
            {
                var result = await _contratacaoService.ListarTemplatesAsync(limite, cursor);
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<List<TemplateDTO>> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpPut("AtualizarTemplate/{id}")]
        public async Task<ActionResult<ApiGenericResult<TemplateDTO>>> AtualizarTemplate(Guid id, [FromBody] TemplateDTO template)
        {
            try
            {
                var result = await _contratacaoService.AtualizarTemplateAsync(id, template, _aspNetUser.GetUsuarioLogado().Cpf);
                
                if (!result.Sucesso)
                {
                    return BadRequest(result);
                }
                
                return Ok(result);
            }
            catch (ApplicationException ex)
            {
                // Exceções de validação - retornar BadRequest
                return BadRequest(new ApiGenericResult<TemplateDTO>
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                });
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<TemplateDTO> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpDelete("DeletarTemplate/{id}")]
        public async Task<ActionResult<ApiGenericResult<bool>>> DeletarTemplate(Guid id)
        {
            try
            {
                var result = await _contratacaoService.DeletarTemplateAsync(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<bool> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ListarLogsTemplate/{templateId}")]
        public async Task<ActionResult<ApiGenericResult<List<TemplateContratacaoLogDTO>>>> ListarLogsTemplate(
            Guid templateId, 
            [FromQuery] int limite = 50, 
            [FromQuery] int cursor = 0)
        {
            try
            {
                var result = await _logService.ListarLogsTemplateAsync(templateId, limite, cursor);
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<List<TemplateContratacaoLogDTO>> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ListarLogsPorColaborador/{colaboradorCodigoInterno}")]
        public async Task<ActionResult<ApiGenericResult<List<TemplateContratacaoLogDTO>>>> ListarLogsPorColaborador(
            string colaboradorCodigoInterno, 
            [FromQuery] int limite = 50, 
            [FromQuery] int cursor = 0)
        {
            try
            {
                var result = await _logService.ListarLogsPorColaboradorAsync(colaboradorCodigoInterno, limite, cursor);
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<List<TemplateContratacaoLogDTO>> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ListarLogsPorPeriodo")]
        public async Task<ActionResult<ApiGenericResult<List<TemplateContratacaoLogDTO>>>> ListarLogsPorPeriodo(
            [FromQuery] DateTime dataInicio, 
            [FromQuery] DateTime dataFim, 
            [FromQuery] int limite = 100, 
            [FromQuery] int cursor = 0)
        {
            try
            {
                var result = await _logService.ListarLogsPorPeriodoAsync(dataInicio, dataFim, limite, cursor);
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<List<TemplateContratacaoLogDTO>> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ListarSistemasLiberados")]
        public async Task<ActionResult<ApiGenericResult<List<SistemaLiberadoDTO>>>> ListarSistemasLiberados()
        {
            try
            {
                var result = await _sistemasLiberadosService.ListarSistemasLiberadosAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<List<SistemaLiberadoDTO>> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ObterSistemaLiberado/{id}")]
        public async Task<ActionResult<ApiGenericResult<SistemaLiberadoDTO>>> ObterSistemaLiberado(Guid id)
        {
            try
            {
                var result = await _sistemasLiberadosService.ObterSistemaLiberadoPorIdAsync(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<SistemaLiberadoDTO> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ListarDiretorios")]
        public async Task<ActionResult<ApiGenericResult<List<DiretorioDTO>>>> ListarDiretorios()
        {
            try
            {
                var result = await _diretoriosService.ListarDiretoriosAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<List<DiretorioDTO>> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ObterDiretorio/{id}")]
        public async Task<ActionResult<ApiGenericResult<DiretorioDTO>>> ObterDiretorio(Guid id)
        {
            try
            {
                var result = await _diretoriosService.ObterDiretorioPorIdAsync(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<DiretorioDTO> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ListarGruposEmails")]
        public async Task<ActionResult<ApiGenericResult<List<GrupoEmailDTO>>>> ListarGruposEmails()
        {
            try
            {
                var result = await _gruposEmailsService.ListarGruposEmailsAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<List<GrupoEmailDTO>> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ObterGrupoEmail/{id}")]
        public async Task<ActionResult<ApiGenericResult<GrupoEmailDTO>>> ObterGrupoEmail(Guid id)
        {
            try
            {
                var result = await _gruposEmailsService.ObterGrupoEmailPorIdAsync(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<GrupoEmailDTO> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ListarEquipamentosPadroes")]
        public async Task<ActionResult<ApiGenericResult<List<EquipamentoPadraoDTO>>>> ListarEquipamentosPadroes()
        {
            try
            {
                var result = await _contratacaoService.ListarEquipamentosPadroesAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<List<EquipamentoPadraoDTO>> 
                { 
                    Sucesso = false, 
                });
            }
        }

        [HttpGet("ListarEquipamentosPadroesAninhados")]
        public async Task<ActionResult<ApiGenericResult<List<EquipamentoPadraoAninhadoDTO>>>> ListarEquipamentosPadroesAninhados()
        {
            try
            {
                var result = await _contratacaoService.ListarEquipamentosPadroesAninhadosAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<List<EquipamentoPadraoAninhadoDTO>> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpGet("ExisteEmailUsuario")]
        public async Task<ActionResult<ApiGenericResult<bool>>> ExisteEmailUsuario([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new ApiGenericResult<bool> 
                    { 
                        Sucesso = false, 
                        Mensagem = "Email é obrigatório" 
                    });
                }

                var result = await _contratacaoService.ExisteEmailUsuarioAsync(email);
                return Ok(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(500, new ApiGenericResult<bool> 
                { 
                    Sucesso = false, 
                    Mensagem = e.Message 
                });
            }
        }

        [HttpPost("EnviarEmailTemplateCandidato")]
        public async Task<ActionResult<ApiGenericResult<bool>>> EnviarEmailTemplateCandidato([FromForm] string param, [FromForm] DocumentoCurriculoParam paramCV = default!)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                // Deserializar o JSON do parâmetro string
                var paramObj = Newtonsoft.Json.JsonConvert.DeserializeObject<EnviarEmailTemplateCandidatoParam>(param);

                byte[] arquivoBytes = null;

                string nomePDF = string.Empty;

                if (paramObj.Anexo)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await paramCV.File.CopyToAsync(memoryStream);
                        arquivoBytes = memoryStream.ToArray();
                        nomePDF = paramCV.File.FileName;
                    }
                }

                var ret = await _contratacaoService.EnviarEmailTemplateCandidato(
                    paramObj.IdCandidatura, 
                    arquivoBytes, 
                    _aspNetUser.GetUsuarioLogado().OrgId, 
                    paramObj.Anexo, 
                    nomePDF,
                    paramObj.EmailsAdicionais,
                    paramObj.OcultarValores);

                if (ret == false)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Erro ao enviar email";
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
                }
                result.Sucesso = true;
                result.Mensagem = "Email enviado com Sucesso";
                return StatusCode(StatusCodes.Status200OK, result);
            }
            catch (UnauthorizedAccessException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }
    }
}
