using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.API.DTOs;
using Colaborador.Domain.Interfaces.Services;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Candidato;
using DataTransferObject.Domain.Candidatura;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.MapaDeAlocacao.GestaoDeAlocados;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Vaga;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using SRS.API.DTOs;
using SRS.Domain.Impl.Service;
using SRS.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SRS.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class CandidaturaController : ControllerBase
    {
        private readonly ICandidaturaService _candidaturaService;
        private readonly UsuarioLogadoDTO _usuarioLogadoDTO;
        private readonly ILogCore _log;
        private readonly IAspNetUser _aspNetUser;
        private readonly IColaboradorService _colaboradorService;

        public CandidaturaController(ICandidaturaService candidaturaService, IAspNetUser aspNetUser, ILogCore log, IColaboradorService colaboradorService)
        {
            _candidaturaService = candidaturaService;
            _aspNetUser = aspNetUser;
            _usuarioLogadoDTO = aspNetUser.GetUsuarioLogado();
            _log = log;
            _colaboradorService = colaboradorService;
        }

        [HttpPost("InserirArquivo")]
        public async Task<ActionResult<CandidaturaArquivosDTO>> InserirArquivo()
        {
            try
            {
                StringValues arquivoParam;
                Request.Form.TryGetValue("arquivoParam", out arquivoParam);


                CandidaturaArquivosParams param = JsonConvert.DeserializeObject<CandidaturaArquivosParams>(arquivoParam);
                byte[] imagem = null;
                var filePath = Path.GetTempFileName();
                var fileType = ".pdf";

                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
                            if (!String.IsNullOrEmpty(formFile.ContentType))
                            {
                                switch (formFile.ContentType)
                                {
                                    // Imagens
                                    case "image/jpg":
                                    case "image/jpeg":
                                        fileType = ".jpg";
                                        break;

                                    case "image/png":
                                        fileType = ".png";
                                        break;

                                    // PDF
                                    case "application/pdf":
                                        fileType = ".pdf";
                                        break;

                                    // Word antigos e novos
                                    case "application/msword":
                                        fileType = ".doc";
                                        break;

                                    case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                                        fileType = ".docx";
                                        break;

                                    // Excel
                                    case "application/vnd.ms-excel":
                                        fileType = ".xls";
                                        break;

                                    case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                                        fileType = ".xlsx";
                                        break;

                                    case "application/vnd.ms-excel.sheet.macroEnabled.12":
                                        fileType = ".xlsm";
                                        break;

                                    case "application/vnd.ms-excel.sheet.binary.macroEnabled.12":
                                        fileType = ".xlsb";
                                        break;

                                    // CSV
                                    case "text/csv":
                                        fileType = ".csv";
                                        break;

                                    // RTF
                                    case "application/rtf":
                                        fileType = ".rtf";
                                        break;

                                    // ODT (OpenDocument Text)
                                    case "application/vnd.oasis.opendocument.text":
                                        fileType = ".odt";
                                        break;

                                    default:
                                        // fallback se não reconhecer
                                        fileType = Path.GetExtension(formFile.FileName)?.ToLower();
                                        break;

                                }
                            }
                            await formFile.CopyToAsync(inputStream);
                            imagem = new byte[inputStream.Length];
                            param.NomeArquivo = formFile.FileName;
                            param.TipoArquivo = fileType;
                            inputStream.Seek(0, SeekOrigin.Begin);
                            inputStream.Read(imagem, 0, imagem.Length);
                        }
                    }
                }
                param.bytes = imagem;
                var codigoInternoColaboradorLogado = _usuarioLogadoDTO.Cpf;
                var result = await _candidaturaService.InserirArquivo(param, codigoInternoColaboradorLogado);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { sucesso = false, mensagem = e.Message });
            }
        }

        [HttpDelete("deletarArquivo/{arquivoPath}")]
        public async Task<ActionResult<ApiGenericResult<StatusResult>>> deletarArquivo(string arquivoPath)
            => Ok(await _candidaturaService.DeletarArquivo(arquivoPath));

        [HttpPost("AtualizarStatusCandidatura")]
        public async Task<ActionResult<ApiGenericResult>> AtualizarStatusCandidatura(AtualizarCandidaturaVagaParam param)
        {
            var result = new ApiGenericResult();
            try
            {
                await _candidaturaService.AtualizarStatusCandidatura(param.VagaId, param.CandidatoId, param.StatusId, param.Descricao, param.Origem);
                return result;
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarStatusCandidatura")]
        public async Task<ActionResult<ApiGenericResult<List<CandidaturaStatusDTO>>>> ListarStatusCandidatura()
        {
            var result = new ApiGenericResult<List<CandidaturaStatusDTO>>();
            try
            {
                result.Retorno = await _candidaturaService.ListarStatusCandidatura();
                return result;
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("ListarCandidatosDeVagas")]
        public async Task<ActionResult<ApiGenericResult<List<VagaCandidatoGestaoDTO>>>> ListarCandidatosDeVagas(ListarVagasCandidatoGestaoParam param)
        {
            var result = new ApiGenericResult<List<VagaCandidatoGestaoDTO>>();
            try
            {
                result.Retorno = await _candidaturaService.ListarCandidatosDeVagas(_usuarioLogadoDTO.Cpf, param.Pesquisa, param.Colaborador, param.StatusList, param.Titulo, _usuarioLogadoDTO.OrgId);
                return result;
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("ReprovarCandidatura")]
        public async Task<ActionResult<ApiGenericResult<bool>>> ReprovarCandidatura(ReprovarCandidaturaParam param)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                result.Mensagem = await _candidaturaService.ReprovarCandidatura(_usuarioLogadoDTO.Cpf, param);
                result.Retorno = true;
                result.Sucesso = true;
                return result;
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
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

        [HttpGet("ListarLogsCandidatura/{codColaborador}")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<CandidaturaLogDTO>>>> ListarLogsCandidatura(string codColaborador)
        {
            var result = new ApiGenericResult<IEnumerable<CandidaturaLogDTO>>();
            try
            {
                result.Retorno = await _candidaturaService.ListarLogsCandidaturaPorColaborador(codColaborador);
                return result;
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
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

        [HttpGet("ListarLogsCandidaturaAgrupados")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<CandidaturaAgrupadaDTO>>>> ListarLogsCandidaturaAgrupados(string codColaborador, string busca, string idCandidatura = null)
        {
            var result = new ApiGenericResult<IEnumerable<CandidaturaAgrupadaDTO>>();
            try
            {
                result.Retorno = await _candidaturaService.ListarLogsCandidaturaAgrupadosPorColaborador(codColaborador, busca, idCandidatura);
                return result;
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
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

        [HttpPut("AtualizarCandidatura")]
        public async Task<ActionResult<ApiGenericResult<object>>> AtualizarCandidatura([FromBody] AtualizarCandidaturaParam param)
        {
            var result = new ApiGenericResult<object>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                if (usuarioLogado == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Usuário não autenticado";
                    return StatusCode(401, result);
                }

                await _candidaturaService.AtualizarCandidatura(param, usuarioLogado.Cpf);
                result.Sucesso = true;
                result.Mensagem = "Candidatura atualizada com sucesso";
                return result;
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
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

        /// <summary>Histórico de alterações em pretensão salarial e modelo de trabalho da candidatura (paginado; apenas o recrutador responsável).</summary>
        [HttpGet("ListarLogPretensaoModeloCandidatura")]
        public async Task<ActionResult<ApiGenericResult<List<LogPretensaoModeloCandidaturaDTO>>>> ListarLogPretensaoModeloCandidatura(
            [FromQuery] Guid idCandidatura,
            [FromQuery] int limit = 100,
            [FromQuery] int cursor = 0)
        {
            var result = new ApiGenericResult<List<LogPretensaoModeloCandidaturaDTO>>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                if (usuarioLogado == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Usuário não autenticado";
                    return StatusCode(StatusCodes.Status401Unauthorized, result);
                }

                result.Retorno = await _candidaturaService.ListarLogPretensaoModeloPorCandidatura(idCandidatura, usuarioLogado.Cpf, limit, cursor);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log(e.Message, LevelsEnum.Warning);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                result.Retorno = null;
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log(e.Message, LevelsEnum.Error);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log(e.Message, LevelsEnum.Error);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        /// <summary>Histórico de pretensão/modelo do candidato na organização: usa a candidatura só para autorização (recrutador responsável) e contexto do candidato; retorna logs de todas as candidaturas desse candidato na org (paginado).</summary>
        [HttpGet("ListarLogPretensaoModeloPorOrganizacao")]
        public async Task<ActionResult<ApiGenericResult<List<LogPretensaoModeloCandidaturaDTO>>>> ListarLogPretensaoModeloPorOrganizacao(
            [FromQuery] Guid codigoCandidatura,
            [FromQuery] int limit = 100,
            [FromQuery] int cursor = 0)
        {
            var result = new ApiGenericResult<List<LogPretensaoModeloCandidaturaDTO>>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                if (usuarioLogado == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Usuário não autenticado";
                    return StatusCode(StatusCodes.Status401Unauthorized, result);
                }

                result.Retorno = await _candidaturaService.ListarLogPretensaoModeloPorOrganizacao(codigoCandidatura, usuarioLogado.OrgId, limit, cursor, usuarioLogado.Cpf);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log(e.Message, LevelsEnum.Warning);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                result.Retorno = null;
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log(e.Message, LevelsEnum.Error);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return BadRequest(result);
            }
            catch (Exception e)
            {
                _log.Log(e.Message, LevelsEnum.Error);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPut("AtualizarRecrutadorResponsavel")]
        public async Task<ActionResult<ApiGenericResult<object>>> AtualizarRecrutadorResponsavel(string idCandidatura, string codigoRecrutadorResponsavel)
        {
            var result = new ApiGenericResult<object>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                if (usuarioLogado == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Usuário não autenticado";
                    return StatusCode(401, result);
                }

                await _candidaturaService.AtualizarRecrutadorResponsavel(idCandidatura, codigoRecrutadorResponsavel, usuarioLogado.Cpf);
                result.Sucesso = true;
                result.Mensagem = "Recrutador responsavel atualizado com sucesso";
                return result;
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
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

        [HttpGet("ObterCandidaturaPorId")]
        public async Task<ActionResult<ApiGenericResult<CandidaturaRecrutamentoDTO>>> BuscarCandidatura(string idCandidatura)
        {
            var result = new ApiGenericResult<CandidaturaRecrutamentoDTO>();
            try
            {
                result.Retorno = await _candidaturaService.ObterCandidaturaPorId(idCandidatura);
                return result;
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e) when (e is ArgumentException || e is ApplicationException)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
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

        [HttpGet("GetPdfTemplateHeaderPdfAdmissao")]
        public async Task<ActionResult<ApiGenericResult<PdfTemplateDTO>>> GetPdfTemplateHeaderPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga) =>
            Ok(await _candidaturaService.GetPdfTemplateHeaderPdfAdmissao(idCandidatura, codInternoCandidato, idVaga));

        [HttpGet("GetProfissionalPdfAdmissao")]
        public async Task<ActionResult<ApiGenericResult<ProfissionalDTO>>> GetProfissionalPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga) =>
            Ok(await _candidaturaService.GetProfissionalPdfAdmissao(idCandidatura, codInternoCandidato, idVaga));

        [HttpGet("GetVagaAdmissaoPdfAdmissao")]
        public async Task<ActionResult<ApiGenericResult<VagaAdmissaoDTO>>> GetVagaAdmissaoPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga) =>
            Ok(await _candidaturaService.GetVagaAdmissaoPdfAdmissao(idCandidatura, codInternoCandidato, idVaga));

        [HttpGet("GetBeneficiosPdfAdmissao")]
        public async Task<ActionResult<ApiGenericResult<BeneficiosDTO>>> GetBeneficiosPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga) =>
            Ok(await _candidaturaService.GetBeneficiosPdfAdmissao(idCandidatura, codInternoCandidato, idVaga));

        [HttpGet("GetChecklistInstalacaoPdfAdmissao")]
        public async Task<ActionResult<ApiGenericResult<ChecklistInstalacaoDTO>>> GetChecklistInstalacaoPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga) =>
            Ok(await _candidaturaService.GetChecklistInstalacaoPdfAdmissao(idCandidatura, codInternoCandidato, idVaga));

        [HttpGet("GetAcessosUsuarioPdfAdmissao")]
        public async Task<ActionResult<ApiGenericResult<AcessosUsuarioDTO>>> GetAcessosUsuarioPdfAdmissao(string idCandidatura, string codInternoCandidato, string idVaga) =>
            Ok(await _candidaturaService.GetAcessosUsuarioPdfAdmissao(idCandidatura, codInternoCandidato, idVaga));

        [HttpGet("GetPdfTemplateDeAdmissaoPdf")]
        public async Task<ActionResult<ApiGenericResult<PdfTemplateDTO>>> GetPdfTemplateDeAdmissaoPdf(string idCandidatura, string codInternoCandidato, string idVaga) =>
            Ok(await _candidaturaService.GetPdfTemplateDeAdmissaoPdf(idCandidatura, codInternoCandidato, idVaga));

        [HttpGet("GetEquipamentoFourSysAdmissaoPdf")]
        public async Task<ActionResult<ApiGenericResult<EquipamentosFoursysDTO>>> GetEquipamentoFourSysAdmissaoPdf(string idCandidatura, string codInternoCandidato, string idVaga) =>
            Ok(await _candidaturaService.GetEquipamentoFourSysAdmissaoPdf(idCandidatura, codInternoCandidato, idVaga));

        [HttpPost("CreatePdfTemplateDeAdmissao")]
        public async Task<ActionResult<ApiGenericResult<PdfTemplateDTO>>> CreatePdfTemplateDeAdmissao([FromBody] CreatePdfTemplateParameters param) =>
            Ok(await _candidaturaService.CreatePdfTemplateDeAdmissao(param));

        [HttpGet("GetSistemasLiberadosTemplatePdf")]
        public async Task<ActionResult<ApiGenericResult<AcessoSistemaDTO>>> GetSistemasLiberadosTemplatePdf() =>
            Ok(await _candidaturaService.GetSistemasLiberadosTemplatePdf());

        [HttpGet("GetAcessosPastaRedeTemplatePdf")]
        public async Task<ActionResult<ApiGenericResult<AcessoDiretorioRedeDTO>>> GetAcessosPastaRedeTemplatePdf() =>
            Ok(await _candidaturaService.GetAcessosPastaRedeTemplatePdf());

        [HttpGet("ListarMotivosDeclinio")]
        public async Task<ActionResult<ApiGenericResult<List<MotivoDeclinioDTO>>>> ListarMotivosDeclinio()
        {
            var result = new ApiGenericResult<List<MotivoDeclinioDTO>>();
            try
            {
                result.Retorno = await _candidaturaService.ListarMotivosDeclinio();
                result.Sucesso = true;
                return result;
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarMotivosReprovacao")]
        public async Task<ActionResult<ApiGenericResult<List<MotivoReprovacaoDTO>>>> ListarMotivosReprovacao()
        {
            var result = new ApiGenericResult<List<MotivoReprovacaoDTO>>();
            try
            {
                result.Retorno = await _candidaturaService.ListarMotivosReprovacao();
                result.Sucesso = true;
                return result;
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("DeclinarCandidato")]
        public async Task<ActionResult<ApiGenericResult>> DeclinarCandidato([FromBody] DeclinarCandidatoParam param)
        {
            var result = new ApiGenericResult();
            try
            {
                result.Mensagem = await _candidaturaService.DeclinarCandidato(param, _aspNetUser.GetUsuarioLogado().Cpf);
                result.Sucesso = true;
                return result;
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("ObterDadosParaEditarCandidaturaColaboradorComDadosDemograficos")]
        public async Task<ActionResult<ApiGenericResult<EditarColaboradorComDadosDemograficosResponseDTO>>> ObterDadosParaEditarCandidaturaColaboradorComDadosDemograficos(
            [FromQuery] string codigoInternoColaborador,
            [FromQuery] string idCandidatura = null)
        {
            var result = new ApiGenericResult<EditarColaboradorComDadosDemograficosResponseDTO>();
            try
            {
                if (string.IsNullOrWhiteSpace(codigoInternoColaborador))
                {
                    result.Sucesso = false;
                    result.Mensagem = "O código interno do colaborador é obrigatório.";
                    return BadRequest(result);
                }

                // Buscar dados do colaborador
                var colaboradorDTO = await _colaboradorService.ObterColaboradorPorCodigoAsync(codigoInternoColaborador);
                if (colaboradorDTO == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = $"Colaborador com código {codigoInternoColaborador} não encontrado.";
                    return NotFound(result);
                }

                // Buscar dados demográficos
                var dadosDemograficos = await _colaboradorService.ObterDadosDemograficos(codigoInternoColaborador);

                // Se não houver dados demográficos, criar objeto com valores padrão (mesmo comportamento de ObterDadosDemograficos)
                if (dadosDemograficos == null)
                {
                    dadosDemograficos = new DadosDemograficosColaboradorDTO
                    {
                        QuantidadePessoasResidencia = 1,
                        DependentesIRPF = 0,
                        PossuiConjuge = false,
                        PossuiFilhos = false,
                        Filhos = new List<FilhoColaboradorDTO>(),
                        PossuiSeguroSaude = false,
                        SeguroSaudePossuiCoparticipacao = false,
                        PossuiInteressePlanoFoursys = false,
                        IncluirDependentesPlanoFoursys = false,
                        QuantidadeDependentesPlanoFoursys = 0,
                        EstudaAtualmente = false,
                        FilhosEstudamAte24Anos = false
                    };
                }

                // Buscar dados da candidatura se idCandidatura foi informado
                SRS.API.DTOs.DadosCandidaturaInputDTO dadosCandidatura = null;
                if (!string.IsNullOrWhiteSpace(idCandidatura))
                {
                    var candidatura = await _candidaturaService.ObterCandidaturaPorId(idCandidatura);
                    if (candidatura != null)
                    {
                        // Validar se a candidatura pertence ao colaborador
                        if (candidatura.CodColaborador != codigoInternoColaborador)
                        {
                            result.Sucesso = false;
                            result.Mensagem = "A candidatura informada não pertence ao colaborador especificado.";
                            return BadRequest(result);
                        }

                        dadosCandidatura = new SRS.API.DTOs.DadosCandidaturaInputDTO
                        {
                            IdCandidatura = candidatura.Id,
                            ModeloTrabalhoId = candidatura.ModeloTrabalhoId,
                            QuantidadeDiasPresencial = candidatura.QuantidadeDiasPresencial,
                            PretencaoSalarial = candidatura.PretencaoSalarial
                        };
                    }
                }

                var candidaturasColaborador = await _candidaturaService.BuscarCandidaturasColaborador(codigoInternoColaborador);

                // Mapear dados para o DTO de resposta
                var response = new EditarColaboradorComDadosDemograficosResponseDTO
                {
                    Colaborador = MapearParaEditarColaboradorInputDTOAPI(colaboradorDTO),
                    DadosDemograficos = dadosDemograficos,
                    DadosCandidatura = dadosCandidatura,
                    Candidaturas = candidaturasColaborador.ToList()
                };

                result.Retorno = response;
                result.Sucesso = true;
                result.Mensagem = "Dados obtidos com sucesso!";

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                result.Sucesso = false;
                result.Mensagem = ex.Message;
                return BadRequest(result);
            }
            catch (ApplicationException ex)
            {
                result.Sucesso = false;
                result.Mensagem = ex.Message;
                return BadRequest(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao buscar dados do colaborador e dados demográficos";
                return StatusCode(500, result);
            }
        }

        [HttpPost("EditarCandidaturaColaboradorComDadosDemograficos")]
        public async Task<ActionResult<ApiGenericResult<EditarColaboradorDTO>>> EditarCandidaturaColaboradorComDadosDemograficos([FromBody] EditarColaboradorComDadosDemograficosParam param)
        {
            var result = new ApiGenericResult<EditarColaboradorDTO>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                // Mapear DTOs da camada API para DTOs da camada Domain
                var colaboradorInputDTO = MapearParaEditarColaboradorInputDTO(param.Colaborador);
                var dadosCandidaturaDTO = param.DadosCandidatura != null ? MapearParaDadosCandidaturaInputDTO(param.DadosCandidatura) : null;

                var colaboradorAtualizado = await _colaboradorService.EditarCandidaturaColaboradorComDadosDemograficos(
                    colaboradorInputDTO,
                    param.DadosDemograficos,
                    dadosCandidaturaDTO,
                    usuarioLogado.Cpf
                );

                result.Retorno = colaboradorAtualizado;
                result.Sucesso = true;
                result.Mensagem = "Dados do colaborador e dados demográficos atualizados com sucesso!";

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                // Exceções de validação - retornar mensagem específica
                result.Sucesso = false;
                result.Mensagem = ex.Message;
                return BadRequest(result);
            }
            catch (ApplicationException ex)
            {
                // Exceções de validação - retornar mensagem específica
                result.Sucesso = false;
                result.Mensagem = ex.Message;
                return BadRequest(result);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(StatusCodes.Status401Unauthorized, result);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                result.Sucesso = false;
                result.Mensagem = "Erro interno ao atualizar dados do colaborador e dados demográficos";
                return StatusCode(500, result);
            }
        }

        /// <summary>
        /// Mapeia EditarColaboradorInputDTO da camada API para EditarColaboradorInputDTO da camada Domain
        /// </summary>
        private DataTransferObject.Domain.Colaborador.EditarColaboradorInputDTO MapearParaEditarColaboradorInputDTO(SRS.API.DTOs.EditarColaboradorInputDTO input)
        {
            if (input == null)
                return null;

            return new DataTransferObject.Domain.Colaborador.EditarColaboradorInputDTO
            {
                CodigoInternoColaborador = input.CodigoInternoColaborador,
                NomeCompleto = input.NomeCompleto,
                DataNascimento = input.DataNascimento,
                Rg = input.Rg,
                Matricula = input.Matricula,
                EnderecoId = input.EnderecoId,
                Ativo = input.Ativo,
                ContatoPrincipalDdi = input.ContatoPrincipalDdi,
                ContatoPrincipal = input.ContatoPrincipal,
                ContatoOutro = input.ContatoOutro,
                ImagemId = input.ImagemId,
                Candidato = input.Candidato,
                Passaporte = input.Passaporte,
                ColaboradorSaudeId = input.ColaboradorSaudeId,
                EstadoCivil = input.EstadoCivil,
                Genero = input.Genero,
                Etnia = input.Etnia,
                OrientacaoSexual = input.OrientacaoSexual,
                Escolaridade = input.Escolaridade,
                Refugiado = input.Refugiado,
                EmailAlternativo = input.EmailAlternativo,
                Nacionalidade = input.Nacionalidade,
                Sobre = input.Sobre,
                DocumentoColaborador = input.DocumentoColaborador,
                UrlLinkedin = input.UrlLinkedin,
                DataSyncLinkedin = input.DataSyncLinkedin,
                VisualizarBuscaAderencia = input.VisualizarBuscaAderencia,
                Qualificado = input.Qualificado,
                Endereco = input.Endereco,
                Saude = input.Saude != null ? MapearParaColaboradorSaudeInputDTO(input.Saude) : null
            };
        }

        /// <summary>
        /// Mapeia ColaboradorSaudeInputDTO da camada API para ColaboradorSaudeInputDTO da camada Domain
        /// </summary>
        private DataTransferObject.Domain.Colaborador.ColaboradorSaudeInputDTO MapearParaColaboradorSaudeInputDTO(SRS.API.DTOs.ColaboradorSaudeInputDTO input)
        {
            if (input == null)
                return null;

            return new DataTransferObject.Domain.Colaborador.ColaboradorSaudeInputDTO
            {
                PCD = input.PCD,
                GrupoDeRiscoCovid = input.GrupoDeRiscoCovid,
                CondicaoDeSaudeRelevante = input.CondicaoDeSaudeRelevante
            };
        }

        /// <summary>
        /// Mapeia DadosCandidaturaInputDTO da camada API para DadosCandidaturaInputDTO da camada Domain
        /// </summary>
        private DataTransferObject.Domain.Colaborador.DadosCandidaturaInputDTO MapearParaDadosCandidaturaInputDTO(SRS.API.DTOs.DadosCandidaturaInputDTO input)
        {
            if (input == null)
                return null;

            return new DataTransferObject.Domain.Colaborador.DadosCandidaturaInputDTO
            {
                IdCandidatura = input.IdCandidatura,
                ModeloTrabalhoId = input.ModeloTrabalhoId,
                QuantidadeDiasPresencial = input.QuantidadeDiasPresencial,
                PretencaoSalarial = input.PretencaoSalarial
            };
        }

        /// <summary>
        /// Mapeia EditarColaboradorDTO da camada Domain para EditarColaboradorInputDTO da camada API
        /// </summary>
        private SRS.API.DTOs.EditarColaboradorInputDTO MapearParaEditarColaboradorInputDTOAPI(DataTransferObject.Domain.Colaborador.EditarColaboradorDTO dto)
        {
            if (dto == null)
                return null;

            return new SRS.API.DTOs.EditarColaboradorInputDTO
            {
                CodigoInternoColaborador = dto.CodigoInternoColaborador,
                NomeCompleto = dto.NomeCompleto,
                DataNascimento = dto.DataNascimento,
                Rg = dto.Rg,
                Matricula = dto.Matricula,
                EnderecoId = dto.EnderecoId,
                Ativo = dto.Ativo,
                ContatoPrincipalDdi = dto.ContatoPrincipalDdi,
                ContatoPrincipal = dto.ContatoPrincipal,
                ContatoOutro = dto.ContatoOutro,
                ImagemId = dto.ImagemId,
                Candidato = dto.Candidato,
                Passaporte = dto.Passaporte,
                ColaboradorSaudeId = dto.ColaboradorSaudeId,
                EstadoCivil = dto.EstadoCivil,
                Genero = dto.Genero,
                Etnia = dto.Etnia,
                OrientacaoSexual = dto.OrientacaoSexual,
                Escolaridade = dto.Escolaridade,
                Refugiado = dto.Refugiado,
                EmailAlternativo = dto.EmailAlternativo,
                Email = dto.Email,
                Nacionalidade = dto.Nacionalidade,
                Sobre = dto.Sobre,
                DocumentoColaborador = dto.DocumentoColaborador,
                UrlLinkedin = dto.UrlLinkedin,
                DataSyncLinkedin = dto.DataSyncLinkedin,
                VisualizarBuscaAderencia = dto.VisualizarBuscaAderencia,
                Qualificado = dto.Qualificado,
                Endereco = dto.Endereco,
                Saude = dto.Saude != null ? MapearParaColaboradorSaudeInputDTOAPI(dto.Saude) : null
            };
        }

        /// <summary>
        /// Mapeia ColaboradorSaudeDTO da camada Domain para ColaboradorSaudeInputDTO da camada API
        /// </summary>
        private SRS.API.DTOs.ColaboradorSaudeInputDTO MapearParaColaboradorSaudeInputDTOAPI(DataTransferObject.Domain.Colaborador.ColaboradorSaudeDTO dto)
        {
            if (dto == null)
                return null;

            return new SRS.API.DTOs.ColaboradorSaudeInputDTO
            {
                PCD = dto.PCD,
                GrupoDeRiscoCovid = dto.GrupoDeRiscoCovid,
                CondicaoDeSaudeRelevante = dto.CondicaoDeSaudeRelevante
            };
        }

        [HttpGet("ListarMeusTalentos")]
        public async Task<ActionResult<ApiGenericResult<List<MeusTalentos>>>> ListarMeusTalentos()
        {
            var result = new ApiGenericResult<List<MeusTalentos>>();
            try
            {
                result.Retorno = await _candidaturaService.ListarMeusTalentos(_usuarioLogadoDTO.Cpf);
                result.Sucesso = true;
                return result;
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpPost("CandidatoTemplateEmailInserir")]
        public async Task<ActionResult<ApiGenericResult<CandidatoTemplateEmailResponseDTO>>> CandidatoTemplateEmailInserir([FromBody] CandidatoTemplateEmailParamDTO param)
            => Ok(await _candidaturaService.CandidatoTemplateEmailInserir(param));

        [HttpPut("CandidatoTemplateEmailAtualizar")]
        public async Task<ActionResult<ApiGenericResult<CandidatoTemplateEmailResponseDTO>>> CandidatoTemplateEmailAtualizar([FromBody] CandidatoTemplateEmailParamDTO param)
            => Ok(await _candidaturaService.CandidatoTemplateEmailAtualizar(param));

        [HttpDelete("CandidatoTemplateEmailDeletar/{orgId}")]
        public async Task<ActionResult<ApiGenericResult<bool>>> CandidatoTemplateEmailDeletar(int orgId)
            => Ok(await _candidaturaService.CandidatoTemplateEmailDeletar(orgId));

        [HttpGet("CandidatoTemplateEmailListarPorId")]
        public async Task<ActionResult<ApiGenericResult<CandidatoTemplateEmailResponseDTO>>> CandidatoTemplateEmailListarPorId(int orgId)
        {
            var result = new ApiGenericResult<CandidatoTemplateEmailResponseDTO>();
            try
            {
                result.Retorno = await _candidaturaService.CandidatoTemplateEmailListarPorId(orgId);
                result.Sucesso = true;
                return result;
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message;
                return StatusCode(500, result);
            }
        }

        [HttpGet("BuscarCandidaturasColaborador")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<ListarCandidaturasPorCodCandidatoResult>>>> BuscarCandidaturasColaborador([FromQuery] string codColaborador)
        {
            var result = new ApiGenericResult<IEnumerable<ListarCandidaturasPorCodCandidatoResult>>();
            try
            {
                if (string.IsNullOrWhiteSpace(codColaborador))
                {
                    result.Sucesso = false;
                    result.Mensagem = "O código do colaborador é obrigatório.";
                    return BadRequest(result);
                }

                result.Retorno = await _candidaturaService.BuscarCandidaturasColaborador(codColaborador);
                result.Sucesso = true;
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                result.Sucesso = false;
                result.Mensagem = ex.Message;
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

        [HttpGet("dashboardMetricasRecrutamento")]
        public async Task<ActionResult<ApiGenericResult<DashboardBigNumbers>>> dashboardMetricasRecrutamento([FromQuery] DashboardBigNumbersParam param)
        {
            ApiGenericResult<DashboardBigNumbers> result = await _candidaturaService.dashboardMetricasRecrutamento(param);

            return Ok(result);
        }

        [HttpGet("dashboardMetricasVagasEmFoco")]
        public async Task<ActionResult<ApiGenericResult<List<DashboardVagasEmFocoResponse>>>> dashboardMetricasVagasEmFoco([FromQuery] DashboardBigNumbersParam param)
        {
            ApiGenericResult<List<DashboardVagasEmFocoResponse>> result = await _candidaturaService.dashboardMetricasVagasEmFoco(param);

            return Ok(result);
        }

         [HttpGet("dashboardMetricasFunilDeVagas")]
        public async Task<ActionResult<ApiGenericResult<DashboardFunilVagasResponse>>> dashboardMetricasFunilDeVagas([FromQuery] DashboardBigNumbersParam param)
        {
            ApiGenericResult<DashboardFunilVagasResponse> result = await _candidaturaService.dashboardMetricasFunilDeVagas(param);

            return Ok(result);
        }

        [HttpGet("dashboardMetricasVagasPerdidasMotivo")]
        public async Task<ActionResult<ApiGenericResult<List<DashboardPerdidasMotivoResponse>>>> dashboardMetricasVagasPerdidasMotivo([FromQuery] DashboardBigNumbersParam param)
        {
            ApiGenericResult<List<DashboardPerdidasMotivoResponse>> result = await _candidaturaService.dashboardMetricasVagasPerdidasMotivo(param);

            return Ok(result);
        }
        /*
        [HttpGet("dashboardMetricasAquisicaoCandidatos")]
        public async Task<ActionResult<ApiGenericResult<List<DashboardAquisicaoCandidatos>>>> dashboardMetricasAquisicaoCandidatos([FromQuery] DashboardBigNumbersParam param)
        {
            ApiGenericResult<List<DashboardAquisicaoCandidatos>> result = await _candidaturaService.dashboardMetricasAquisicaoCandidatos(param);

            return Ok(result);
        }
        */
        [HttpGet("dashboardNovosCandidatosPorOrigem")]
        public async Task<ActionResult<ApiGenericResult<List<DashboardAquisicaoCandidatosResponse>>>> dashboardNovosCandidatosPorOrigem([FromQuery] DashboardNovosCandidatosParam param)
        {
            ApiGenericResult<List<DashboardAquisicaoCandidatosResponse>> result = await _candidaturaService.dashboardNovosCandidatosPorOrigem(param);
            return Ok(result);
        }

        [HttpGet("RecrutadorListagem")]
        public async Task<ActionResult<ApiGenericResult<List<RecrutadorListagemResponse>>>> RecrutadorListagem(string nomeRecrutador, int cursor, int limite)
        {
            ApiGenericResult<List<RecrutadorListagemResponse>> result = await _candidaturaService.RecrutadorListagem(nomeRecrutador, cursor, limite);
            return Ok(result);
        }
    }
}