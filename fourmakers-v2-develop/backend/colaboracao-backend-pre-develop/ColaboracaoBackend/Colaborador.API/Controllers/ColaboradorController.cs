using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Colaborador.API.DTOs;
using Colaborador.Domain.Interfaces.Services;
using Colaborador.Domain.Interfaces.Services.BancoDeTalentos;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Comentario;
using DataTransferObject.Domain.LG.Holerite;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObject.Domain.Foursys;

namespace Colaborador.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [LogAction]
    public class ColaboradorController : Controller
    {
        private readonly IColaboradorService _colaboradorService;
        private readonly IAspNetUser _aspNetUser;
        private readonly ILogCore _log;
        private readonly IConfiguration _configuration;
        private readonly IBancoDeTalentosService _bancoDeTalentosService;

        public ColaboradorController(IColaboradorService colaboradorService, IAspNetUser aspNetUser, ILogCore log, IConfiguration configuration, IBancoDeTalentosService bancoDeTalentosService)
        {
            _colaboradorService = colaboradorService;
            _aspNetUser = aspNetUser;
            _log = log;
            _configuration = configuration;
            _bancoDeTalentosService = bancoDeTalentosService;
        }

        [HttpPost("InserirColaborador")]
        public async Task<ActionResult<ApiGenericResult<string>>> InserirColaborador([FromBody] CadastroColaboradorInput input)
        {
            var ret = new ApiGenericResult<string>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                ret = await _colaboradorService.InserirColaborador(input, usuarioLogado.Cpf, usuarioLogado.OrgId);
                if (ret.Sucesso)
                {
                    ret.Mensagem = "Colaborador cadastrado com sucesso!";
                    return Ok(ret);
                }
                else
                {
                    return BadRequest(ret);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                _log.Log("UNAUTHORIZED_EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Critical);
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<string> { Sucesso = false, Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Error);
                _log.Log(ex.StackTrace, LevelsEnum.Error);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiGenericResult<string> { Sucesso = false, Mensagem = ex.Message });
            }
        }

        [HttpPost("EditarColaborador")]
        public async Task<ActionResult<ApiGenericResult<string>>> EditarColaborador([FromBody] CadastroColaboradorInput input)
        {
            var ret = new ApiGenericResult<string>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                ret = await _colaboradorService.EditarColaborador(input, usuarioLogado.Cpf, usuarioLogado.OrgId);
                if (ret.Sucesso)
                {
                    ret.Mensagem = "Colaborador atualizado com sucesso!";
                    return Ok(ret);
                }
                else
                {
                    return BadRequest(ret);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<string> { Sucesso = false, Mensagem = "Acesso não autorizado!" });
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("BuscarDadosColaborador")]
        public async Task<ActionResult<ColaboradorResult>> BuscarDadosColaborador(string cpf)
        {
            var ret = new ColaboradorResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                if (string.IsNullOrEmpty(cpf))
                {
                    cpf = usuarioLogado.Cpf;
                }

                ret.Colaborador = _colaboradorService.BuscarDadosColaborador(cpf, usuarioLogado.OrgId, usuarioLogado.Cpf, usuarioLogado.Token);
                ret.PerfilProfissional = await _colaboradorService.BuscarPerfilProfissional(cpf);
                ret.Colaborador.QuemCadastrou = await _bancoDeTalentosService.BuscarNomeCadastrante(cpf, usuarioLogado.OrgId);

                return ret;
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = ("Acesso não autorizado!");
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("ListarDadosDoColaboradorPorIds")]
        public async Task<ActionResult<ApiGenericResult<List<DadosColaboradorDTO>>>> ListarDadosDoColaboradorPorIds(ListarDadosDoColaboradorPorCpfsInput input)
        {
            var ret = new ApiGenericResult<List<DadosColaboradorDTO>>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                var result = await _colaboradorService.ListarDadosDoColaboradorPorIds(input.Cpfs, usuarioLogado.OrgId, usuarioLogado.Cpf);
                ret.Retorno = result;
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("BuscarColaborador")]
        public BuscaColaboradorResult BuscarColaborador(string cpf, string busca, int cursor, int limite)
        {
            var ret = new BuscaColaboradorResult();
            try
            {
                int filteredResultCount = 0;
                int totalResultCount = 0;
                ret.Colaboradores = _colaboradorService.BuscarColaborador(cpf, _aspNetUser.GetUsuarioLogado().OrgId, busca, cursor, limite, out filteredResultCount, out totalResultCount);
                ret.FilteredResultCount = filteredResultCount;
                ret.TotalResultCount = totalResultCount;
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                var tempAux = e.InnerException;
                while (tempAux != null)
                {
                    ret.Mensagem += "\n" + tempAux.Message;
                    tempAux = tempAux.InnerException;
                }

                return ret;
            }
        }

        [HttpGet("BuscarForcaPerfil")]
        public async Task<ForcaPerfilResult> BuscarForcaPerfil()
        {
            var ret = new ForcaPerfilResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                ret.ForcaPerfil = await _colaboradorService.BuscarForcaPerfil(usuarioLogado.Cpf, usuarioLogado.OrgId);
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                var tempAux = e.InnerException;
                while (tempAux != null)
                {
                    ret.Mensagem += "\n" + tempAux.Message;
                    tempAux = tempAux.InnerException;
                }

                return ret;
            }
        }

        [HttpGet("BuscarColaboradoresAtivos")]
        public BuscarColaboradoresAtivosResult BuscarColaboradoresAtivos(int cursor, int limite)
        {
            var ret = new BuscarColaboradoresAtivosResult();
            try
            {
                int filteredResultCount = 0;
                int totalResultCount = 0;
                ret.Colaboradores = _colaboradorService.BuscarColaboradoresAtivos(cursor, limite, out filteredResultCount, out totalResultCount);
                ret.FilteredResultCount = filteredResultCount;
                ret.TotalResultCount = totalResultCount;
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                var tempAux = e.InnerException;
                while (tempAux != null)
                {
                    ret.Mensagem += "\n" + tempAux.Message;
                    tempAux = tempAux.InnerException;
                }

                return ret;
            }
        }

        [HttpPost("AlterarFotoColaborador")]
        public async Task<ActionResult<StatusResult>> AlterarFotoColaborador()
        {
            var ret = new StatusResult();
            try
            {
                byte[] imagem = null;
                bool gerarThumb = true;
                var filePath = Path.GetTempFileName();
                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
                            if (formFile.ContentType == "image/jpeg")
                                gerarThumb = false;
                            if (String.IsNullOrEmpty(formFile.ContentType))
                            {
                                await formFile.CopyToAsync(inputStream);
                                imagem = new byte[inputStream.Length];
                                inputStream.Seek(0, SeekOrigin.Begin);
                                inputStream.Read(imagem, 0, imagem.Length);
                            }
                            else
                            {
                                switch (formFile.ContentType)
                                {
                                    case "image/png":
                                    case "image/webp":
                                    case "image/gif":
                                    case "image/bmp":
                                    case "image/avif":
                                    case "image/heic":
                                    case "image/heif":
                                    case "image/jpg":
                                    case "image/jpeg":
                                        await formFile.CopyToAsync(inputStream);
                                        imagem = new byte[inputStream.Length];
                                        inputStream.Seek(0, SeekOrigin.Begin);
                                        inputStream.Read(imagem, 0, imagem.Length);
                                        break;
                                }
                            }
                        }
                    }
                }
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                ret.Sucesso = await _colaboradorService.AlterarFotoColaborador(usuarioLogado.Cpf, usuarioLogado.OrgId, imagem, gerarThumb);

                return Ok(ret);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION: " + e.Message, LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AdicionarCompetenciaColaborador")]
        public async Task<VincularItemPerfilResult> AdicionarCompetenciaColaborador(List<AdicionarRemoverItemParam> param)
        {
            var ret = new VincularItemPerfilResult();
            ret.Respostas = new List<ItemPerfilResult>();
            try
            {
                var dtos = new List<AdicionarRemoverItemDTO>();
                param.ForEach(m => dtos.Add(new AdicionarRemoverItemDTO { Id = m.Id, NivelId = m.NivelId, Cpf = m.Cpf }));
                ret.Respostas = await _colaboradorService.AdicionarCompetenciaColaborador(dtos);
                ret.Sucesso = true;
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AdicionarCertificadoCompetenciaColaborador")]
        public async Task<CertificadoResult> AdicionarCertificadoCompetenciaColaborador()
        {
            var ret = new CertificadoResult();
            var httpRequest = HttpContext.Request;
            try
            {
                StringValues colaborador;
                httpRequest.Form.TryGetValue("competenciaColaboradorId", out colaborador);

                StringValues instituicao;
                httpRequest.Form.TryGetValue("instituicao", out instituicao);

                StringValues dataConclusao;
                httpRequest.Form.TryGetValue("dataConclusao", out dataConclusao);

                StringValues descricao;
                httpRequest.Form.TryGetValue("descricao", out descricao);

                StringValues cargaHoraria;
                httpRequest.Form.TryGetValue("cargaHoraria", out cargaHoraria);

                int cargaHorariaInt;
                if (!int.TryParse(cargaHoraria, out cargaHorariaInt))
                {
                    throw new Exception("Carga horária deve ser registrada com números initeiros");
                }

                var competenciaColaboradorId = long.Parse(colaborador);
                byte[] file = null;
                TipoCertificadoEnum tipo = TipoCertificadoEnum.IMAGEM;

                var filePath = Path.GetTempFileName();
                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
                            switch (formFile.ContentType)
                            {
                                case "image/png":
                                case "image/jpg":
                                case "image/jpeg":
                                    await formFile.CopyToAsync(inputStream);
                                    file = new byte[inputStream.Length];
                                    inputStream.Seek(0, SeekOrigin.Begin);
                                    inputStream.Read(file, 0, file.Length);
                                    break;

                                case "application/pdf":
                                    await formFile.CopyToAsync(inputStream);
                                    file = new byte[inputStream.Length];
                                    inputStream.Seek(0, SeekOrigin.Begin);
                                    inputStream.Read(file, 0, file.Length);
                                    tipo = TipoCertificadoEnum.PDF;
                                    break;

                                default:
                                    throw new Exception("Formato inválido do certificado.");
                            }
                        }
                    }
                }
                DateTime dataConvert = DateTime.ParseExact(dataConclusao, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                ret.Certificado = await _colaboradorService.InserirCertificadoCompetenciaColaborador(_aspNetUser.GetUsuarioLogado().Cpf, competenciaColaboradorId, file, tipo, dataConvert, instituicao, descricao, cargaHorariaInt);

                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("RemoverCertificadoCompetenciaColaborador")]
        public async Task<StatusResult> RemoverCertificadoCompetenciaColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new StatusResult();
            try
            {
                await _colaboradorService.RemoveCertificadoCompetenciaColaborador(_aspNetUser.GetUsuarioLogado().Cpf, param.Id);

                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AlteraCertificadoPrincipalColaborador")]
        public async Task<StatusResult> AlteraCertificadoPrincipalColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new StatusResult();
            try
            {
                await _colaboradorService.AlteraCertificadoPrincipalColaborador(_aspNetUser.GetUsuarioLogado().Cpf, param.Id);

                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }
        [HttpPost("RemoverCompetenciaColaborador")]
        public async Task<StatusResult> RemoverCompetenciaColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new StatusResult();
            try
            {
                return await _colaboradorService.RemoverCompetenciaColaborador(_aspNetUser.GetUsuarioLogado().Token, param.Cpf, param.Id);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AdicionarFormacaoColaborador")]
        public async Task<VincularItemPerfilResult> AdicionarFormacaoColaborador(List<AdicionarRemoverItemParam> param)
        {
            var ret = new VincularItemPerfilResult();
            ret.Respostas = new List<ItemPerfilResult>();
            try
            {
                var dtos = new List<AdicionarRemoverItemDTO>();
                param.ForEach(m => dtos.Add(new AdicionarRemoverItemDTO { Id = m.Id, NivelId = m.NivelId, Cpf = m.Cpf }));
                ret.Respostas = await _colaboradorService.InserirFormacaoColaborador(_aspNetUser.GetUsuarioLogado().Token, dtos);
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AdicionarCertificadoFormacaoColaborador")]
        public async Task<CertificadoResult> AdicionarCertificadoFormacaoColaborador()
        {
            var ret = new CertificadoResult();
            var httpRequest = HttpContext.Request;
            try
            {
                StringValues colaborador;
                httpRequest.Form.TryGetValue("formacaoColaboradorId", out colaborador);
                var formacaoColaboradorId = long.Parse(colaborador);
                byte[] file = null;
                TipoCertificadoEnum tipo = TipoCertificadoEnum.IMAGEM;

                var filePath = Path.GetTempFileName();
                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
                            switch (formFile.ContentType)
                            {
                                case "image/png":
                                case "image/jpg":
                                case "image/jpeg":
                                    await formFile.CopyToAsync(inputStream);
                                    file = new byte[inputStream.Length];
                                    inputStream.Seek(0, SeekOrigin.Begin);
                                    inputStream.Read(file, 0, file.Length);
                                    break;

                                case "application/pdf":
                                    await formFile.CopyToAsync(inputStream);
                                    file = new byte[inputStream.Length];
                                    inputStream.Seek(0, SeekOrigin.Begin);
                                    inputStream.Read(file, 0, file.Length);
                                    tipo = TipoCertificadoEnum.PDF;
                                    break;

                                default:
                                    throw new Exception("Formato inválido do certificado.");
                            }
                        }
                    }
                }

                ret.Certificado = await _colaboradorService.InserirCertificadoFormacaColaborador(_aspNetUser.GetUsuarioLogado().Cpf, formacaoColaboradorId, file, tipo);

                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("RemoverFormacaoColaborador")]
        public async Task<StatusResult> RemoverFormacaoColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new StatusResult();
            try
            {
                return await _colaboradorService.RemoverFormacaoColaborador(_aspNetUser.GetUsuarioLogado().Token, param.Cpf, param.Id);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AdicionarDominioColaborador")]
        public async Task<VincularItemPerfilResult> AdicionarDominioColaborador(List<AdicionarRemoverItemParam> param)
        {
            var ret = new VincularItemPerfilResult();
            ret.Respostas = new List<ItemPerfilResult>();
            try
            {
                var dtos = new List<AdicionarRemoverItemDTO>();
                param.ForEach(m => dtos.Add(new AdicionarRemoverItemDTO { Id = m.Id, NivelId = m.NivelId, Cpf = m.Cpf }));
                ret.Respostas = await _colaboradorService.InserirDominioColaborador(_aspNetUser.GetUsuarioLogado().Token, dtos);

                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("RemoverDominioColaborador")]
        public async Task<StatusResult> RemoverDominioColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new StatusResult();
            try
            {
                return await _colaboradorService.RemoverDominioColaborador(_aspNetUser.GetUsuarioLogado().Token, param.Cpf, param.Id);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AdicionarInteresseColaborador")]
        public async Task<VincularItemPerfilResult> AdicionarInteresseColaborador(List<AdicionarRemoverItemParam> param)
        {
            var ret = new VincularItemPerfilResult();
            try
            {
                var dtos = new List<AdicionarRemoverItemDTO>();
                param.ForEach(m => dtos.Add(new AdicionarRemoverItemDTO { Id = m.Id, NivelId = m.NivelId, Cpf = m.Cpf, InteresseAtivo = m.InteresseAtivo }));
                ret.Respostas = await _colaboradorService.InserirInteresseColaborador(_aspNetUser.GetUsuarioLogado().Token, dtos);

                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("RemoverInteresseColaborador")]
        public async Task<StatusResult> RemoverInteresseColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new StatusResult();
            try
            {
                return await _colaboradorService.RemoverInteresseColaborador(_aspNetUser.GetUsuarioLogado().Token, param.Cpf, param.Id);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AdicionarHobbieColaborador")]
        public async Task<VincularItemPerfilResult> AdicionarHobbieColaborador(List<AdicionarRemoverItemParam> param)
        {
            var ret = new VincularItemPerfilResult();
            try
            {
                var dtos = new List<AdicionarRemoverItemDTO>();
                param.ForEach(m => dtos.Add(new AdicionarRemoverItemDTO { Id = m.Id, NivelId = m.NivelId, Cpf = m.Cpf }));
                ret.Respostas = await _colaboradorService.InserirHobbieColaborador(_aspNetUser.GetUsuarioLogado().Token, dtos);

                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("RemoverHobbieColaborador")]
        public async Task<StatusResult> RemoverHobbieColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new StatusResult();
            try
            {
                return await _colaboradorService.RemoverHobbieColaborador(_aspNetUser.GetUsuarioLogado().Token, param.Cpf, param.Id);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("GetListCommentType")]
        public async Task<ComentarioTipoResult> GetListCommentType()
        {
            var ret = new ComentarioTipoResult();
            try
            {
                ret.ListComentarioTipo = await _colaboradorService.ListarTipoComentarios();
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("SendComment")]
        public async Task<ComentarioResult> SendComment(int type, string texto)
        {
            var ret = new ComentarioResult();
            try
            {
                ret.Comentario = await _colaboradorService.InserirComentario(_aspNetUser.GetUsuarioLogado().Cpf, type, texto);
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AlterarDadosColaboradorParametros")]
        public async Task<StatusResult> AlterarDadosColaborador(AlterarDadosColaboradorParam alterarDadosColaboradorParam)
        {
            var retorno = new StatusResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                retorno.Sucesso = await _colaboradorService.AlterarDadosColaboradorBase(alterarDadosColaboradorParam.ColaboradorDTO, usuarioLogado.OrgId, alterarDadosColaboradorParam.Imagem);
                return retorno;
            }
            catch (Exception ex)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(ex.StackTrace, LevelsEnum.Trace);
                retorno.Sucesso = false;
                retorno.Mensagem = ex.Message;
                return retorno;
            }
        }

        [HttpGet("BuscarCandidato")]
        public BuscaColaboradorResult BuscarCandidato(string cpf, string busca, int cursor, int limite)
        {
            var ret = new BuscaColaboradorResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                int filteredResultCount = 0;
                int totalResultCount = 0;
                ret.Colaboradores = _colaboradorService.BuscarCandidato(cpf, usuarioLogado.OrgId, busca, cursor, limite, out filteredResultCount, out totalResultCount);
                ret.FilteredResultCount = filteredResultCount;
                ret.TotalResultCount = totalResultCount;
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                var tempAux = e.InnerException;
                while (tempAux != null)
                {
                    ret.Mensagem += "\n" + tempAux.Message;
                    tempAux = tempAux.InnerException;
                }

                return ret;
            }
        }

        [HttpGet("BuscarRedeColaborador")]
        public RedeColaboradorResult BuscarRedeColaborador(string cpf)
        {
            var ret = new RedeColaboradorResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                if (string.IsNullOrEmpty(cpf))
                {
                    cpf = usuarioLogado.Cpf;
                }

                ret.RedeColaborador = _colaboradorService.BuscarRedeColaborador(cpf, usuarioLogado.OrgId, usuarioLogado.Cpf, usuarioLogado.Token);

                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("BuscarNomeColaborador")]
        public NomeColaboradorResult BuscarNomeColaborador(string cpfColaborador)
        {
            var ret = new NomeColaboradorResult();
            try
            {
                ret.Colaborador = _colaboradorService.BuscarNomeColaborador(cpfColaborador);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("ColaboradorSimple")]
        public ColaboradorResult BuscarDadosColaboradorSimple(string cpf)
        {
            var ret = new ColaboradorResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                if (string.IsNullOrEmpty(cpf))
                {
                    cpf = usuarioLogado.Cpf;
                }

                ret.Colaborador = _colaboradorService.BuscarDadosColaborador(cpf, usuarioLogado.OrgId, usuarioLogado.Cpf, usuarioLogado.Token);
                //var retService = _colaboradorService.ListarDadosColaboradorDependente();

                //ret.Dependentes = retService.Select(x => new DependentesDTO
                //{
                //    Id = x.DependentesDTO.Id,
                //}).ToList();

                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;

                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AtualizaCompetenciaColaborador")]
        public async Task<ItemPerfilResult> AtualizaCompetenciaColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new ItemPerfilResult();
            try
            {
                var dtos = new AdicionarRemoverItemDTO { Id = param.Id, NivelId = param.NivelId, Cpf = PegarCpfContexto(_aspNetUser.GetUsuarioLogado().Cpf, param.Cpf) };
                ret = await _colaboradorService.AlterarCompetenciaColaborador(dtos);
                ret.Sucesso = true;
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.ItemId = param.Id;
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("MergeCompetenciaColaborador")]
        public async Task<StatusResult> MergeCompetenciaColaboradorAsync(MergeItemPerfilParam param)
        {
            var ret = new StatusResult();
            try
            {
                var dtos = new MergeItemPerfilDTO();
                dtos.cpf = param.Cpf;
                param.Itens.ForEach(m => dtos.itens.Add(new AdicionarRemoverItemDTO { Id = m.Id, NivelId = m.NivelId }));
                return await _colaboradorService.MergeCompetenciaColaborador(dtos);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AtualizaFormacaoColaborador")]
        public async Task<ItemPerfilResult> AtualizaFormacaoColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new ItemPerfilResult();
            try
            {
                var dtos = new AdicionarRemoverItemDTO { Id = param.Id, NivelId = param.NivelId, Cpf = PegarCpfContexto(_aspNetUser.GetUsuarioLogado().Cpf, param.Cpf) };
                ret = await _colaboradorService.AlterarFormacaoColaborador(dtos);
                ret.Sucesso = true;
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.ItemId = param.Id;
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("MergeFormacaoColaborador")]
        public async Task<StatusResult> MergeFormacaoColaborador(MergeItemPerfilParam param)
        {
            var ret = new StatusResult();
            try
            {
                var dtos = new MergeItemPerfilDTO();
                dtos.cpf = param.Cpf;
                param.Itens.ForEach(m => dtos.itens.Add(new AdicionarRemoverItemDTO { Id = m.Id, NivelId = m.NivelId }));
                return await _colaboradorService.MergeFormacaoColaborador(dtos);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AtualizaDominioColaborador")]
        public async Task<ItemPerfilResult> AtualizaDominioColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new ItemPerfilResult();
            try
            {
                var dtos = new AdicionarRemoverItemDTO { Id = param.Id, NivelId = param.NivelId, Cpf = PegarCpfContexto(_aspNetUser.GetUsuarioLogado().Cpf, param.Cpf) };
                ret = await _colaboradorService.AlterarDominioColaborador(dtos);
                ret.Sucesso = true;
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.ItemId = param.Id;
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("MergeDominioColaborador")]
        public async Task<StatusResult> MergeDominioColaborador(MergeItemPerfilParam param)
        {
            var ret = new StatusResult();
            try
            {
                var dtos = new MergeItemPerfilDTO();
                dtos.cpf = param.Cpf;
                param.Itens.ForEach(m => dtos.itens.Add(new AdicionarRemoverItemDTO { Id = m.Id, NivelId = m.NivelId }));
                return await _colaboradorService.MergeDominioColaborador(dtos);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("MergeInteresseColaborador")]
        public async Task<StatusResult> MergeInteresseColaborador(MergeItemPerfilParam param)
        {
            var ret = new StatusResult();
            try
            {
                var dtos = new MergeItemPerfilDTO();
                dtos.cpf = param.Cpf;
                param.Itens.ForEach(m => dtos.itens.Add(new AdicionarRemoverItemDTO { Id = m.Id, NivelId = m.NivelId }));
                return await _colaboradorService.MergeInteresseColaborador(dtos);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("MergeHobbieColaborador")]
        public async Task<StatusResult> MergeHobbieColaborador(MergeItemPerfilParam param)
        {
            var ret = new StatusResult();
            try
            {
                var dtos = new MergeItemPerfilDTO();
                dtos.cpf = param.Cpf;
                param.Itens.ForEach(m => dtos.itens.Add(new AdicionarRemoverItemDTO { Id = m.Id, NivelId = m.NivelId }));
                return await _colaboradorService.MergeHobbieColaborador(dtos);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("BuscarListaColaboradores")]
        public ListColaboradoresResult BuscarListaColaboradores(string nomeCompleto)
        {
            var ret = new ListColaboradoresResult();
            try
            {
                return _colaboradorService.BuscarListaColaboradores(nomeCompleto);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        private string PegarCpfContexto(string cpfRequest, string cpf)
        {
            return _configuration["Clients:Colaborador:CpfAdmin"] == cpfRequest ? cpf : cpfRequest;
        }

        [HttpGet("BuscarStatusColaborador")]
        public ActionResult<List<ListStatusColaboradorResult>> BuscarStatusColaborador()
        {
            var ret = new List<ListStatusColaboradorResult>();
            try
            {
                var retService = _colaboradorService.BuscarStatusColaborador();

                foreach (var item in retService)
                {
                    var objeto = new ListStatusColaboradorResult();
                    objeto.Id = item.Id;
                    objeto.Descricao = item.Descricao;
                    ret.Add(objeto);
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AdicionarSoftskillColaborador")]
        public async Task<VincularItemPerfilResult> AdicionarSoftskillColaborador(List<AdicionarRemoverItemParam> param)
        {
            var ret = new VincularItemPerfilResult();
            ret.Respostas = new List<ItemPerfilResult>();
            try
            {
                var dtos = new List<AdicionarRemoverItemDTO>();
                param.ForEach(m => dtos.Add(new AdicionarRemoverItemDTO { Id = m.Id, NivelId = m.NivelId, Cpf = m.Cpf }));
                ret.Respostas = await _colaboradorService.InserirSoftskillColaborador(_aspNetUser.GetUsuarioLogado().Token, dtos);

                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }
        [HttpPost("RemoverSoftskillColaborador")]
        public async Task<StatusResult> RemoverSoftskillColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new StatusResult();
            try
            {
                return await _colaboradorService.RemoverSoftskillColaborador(_aspNetUser.GetUsuarioLogado().Token, param.Cpf, param.Id);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }
        [HttpPost("AtualizaSoftskillColaborador")]
        public async Task<ItemPerfilResult> AtualizaSoftskillColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new ItemPerfilResult();
            try
            {
                var dtos = new AdicionarRemoverItemDTO { Id = param.Id, NivelId = param.NivelId, Cpf = PegarCpfContexto(_aspNetUser.GetUsuarioLogado().Cpf, param.Cpf) };
                ret = await _colaboradorService.AlterarSoftskillColaborador(dtos);
                ret.Sucesso = true;
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.ItemId = param.Id;
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("InserirContatoDeEmergencia")]
        public async Task<IActionResult> InserirContatoDeEmergencia(ContatoEmergenciaDTO param)
        {
            var ret = new StatusResult();
            try
            {
                await _colaboradorService.InserirContatoEmergencia(param, _aspNetUser.GetUsuarioLogado().Cpf);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarContatoDeEmergencia")]
        public async Task<IActionResult> ListarContatoDeEmergencia()
        {
            var ret = new ContatosEmergenciaResult();
            try
            {
                ret.contatos = await _colaboradorService.ListarContatoEmergencia(_aspNetUser.GetUsuarioLogado().Cpf);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AlterarContatoDeEmergencia")]
        public async Task<IActionResult> AlterarContatoDeEmergencia(ContatoEmergenciaDTO param)
        {
            var ret = new StatusResult();
            try
            {
                await _colaboradorService.AlterarContatoEmergencia(param, _aspNetUser.GetUsuarioLogado().Cpf);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("RemoverContatoDeEmergencia")]
        public async Task<IActionResult> RemoverContatoDeEmergencia(RemoverContatoDeEmergenciaParam param)
        {
            var ret = new StatusResult();
            try
            {
                await _colaboradorService.DeletarContatoEmergencia(param.contact_order, _aspNetUser.GetUsuarioLogado().Cpf);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("InserirDadosPCD")]
        public async Task<IActionResult> InserirDadosPCD(InserirDadosPCDParam param)
        {
            var ret = new StatusResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                await _colaboradorService
                    .InserirDadosPCD(param.PCD, usuarioLogado.OrgId,
                    param.grupoDeRisco,
                    param.DescricaoCondicaoDeSaude,
                    usuarioLogado.Cpf);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AlterarDadosPCD")]
        public async Task<IActionResult> AlterarDadosPCD(InserirDadosPCDParam param)
        {
            var ret = new StatusResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                await _colaboradorService
                    .AlterarDadosPCD(param.PCD, usuarioLogado.OrgId,
                    param.grupoDeRisco,
                    param.DescricaoCondicaoDeSaude,
                    usuarioLogado.Cpf);
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("InserirDadosDemograficos")]
        public async Task<IActionResult> InserirDadosDemograficos([FromBody] InserirDadosDemograficosParam param)
        {
            var ret = new StatusResult();
            try
            {
                if (string.IsNullOrEmpty(param.CodigoInternoColaborador))
                    throw new ArgumentException("Código interno do colaborador é obrigatório.");

                await _colaboradorService.InserirDadosDemograficos(param.DadosDemograficos, param.CodigoInternoColaborador);
                ret.Sucesso = true;
                ret.Mensagem = "Dados demográficos inseridos com sucesso!";
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AlterarDadosDemograficos")]
        public async Task<IActionResult> AlterarDadosDemograficos([FromBody] InserirDadosDemograficosParam param)
        {
            var ret = new StatusResult();
            try
            {
                if (string.IsNullOrEmpty(param.CodigoInternoColaborador))
                    throw new ArgumentException("Código interno do colaborador é obrigatório.");

                await _colaboradorService.AlterarDadosDemograficos(param.DadosDemograficos, param.CodigoInternoColaborador);
                ret.Sucesso = true;
                ret.Mensagem = "Dados demográficos atualizados com sucesso!";
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ObterDadosDemograficos")]
        public async Task<ActionResult<ApiGenericResult<DadosDemograficosColaboradorDTO>>> ObterDadosDemograficos([FromQuery] string codigoInternoColaborador)
        {
            var ret = new ApiGenericResult<DadosDemograficosColaboradorDTO>();
            try
            {
                if (string.IsNullOrEmpty(codigoInternoColaborador))
                    throw new ArgumentException("Código interno do colaborador é obrigatório.");

                ret.Retorno = await _colaboradorService.ObterDadosDemograficos(codigoInternoColaborador);
                ret.Sucesso = true;
                ret.Mensagem = "Dados demográficos obtidos com sucesso!";
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("BuscarFormularioColaborador")]
        public ColaboradorResult BuscarFormularioColaborador()
        {
            var ret = new ColaboradorResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                ret.Colaborador = _colaboradorService.BuscarDadosColaborador(usuarioLogado.Cpf, usuarioLogado.OrgId, usuarioLogado.Cpf, usuarioLogado.Token);
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;

                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("GetFormularioColaboradorSimples")]
        public async Task<FormularioColaboradorResult> GetFormularioColaboradorSimples()
        {
            var ret = new FormularioColaboradorResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                var colaborador = _colaboradorService.BuscarDadosColaborador(usuarioLogado.Cpf, usuarioLogado.OrgId, usuarioLogado.Cpf, usuarioLogado.Token);
                ret.Formulario = new FormularioColaborador
                {
                    cpf = colaborador.Cpf,
                    celular = colaborador.ContatoPrincipal,
                    data_nascimento = colaborador.DataNascimento,
                    documentoColaborador = colaborador.DocumentoColaborador,
                    email = colaborador.Email,
                    emailAlternativo = colaborador.EmailAlternativo,
                    endereco = colaborador.Endereco,
                    escolaridade = colaborador.Escolaridade,
                    estado_civil = colaborador.EstadoCivil,
                    etnia = colaborador.Etnia,
                    genero = colaborador.Genero,
                    nome = colaborador.NomeCompleto,
                    orientacao_sexual = colaborador.OrientacaoSexual,
                    passaporte = colaborador.Passaporte,
                    pessoa_refugiada = colaborador.PessoaRefugiada,
                    rg = colaborador.Rg,
                    saude = colaborador.Saude
                };

                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;

                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AlterarFormularioColaborador")]
        public async Task<StatusResult> AlterarFormularioColaborador(FormularioColaborador formulario)
        {
            var ret = new StatusResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                await _colaboradorService.AlterarFormularioColaborador(formulario.nome, formulario.data_nascimento, usuarioLogado.Cpf, formulario.rg, formulario.estado_civil, formulario.escolaridade
                    , formulario.etnia, formulario.genero, formulario.orientacao_sexual, formulario.pessoa_refugiada ?? false, formulario.email, formulario.emailAlternativo, formulario.celular,
                    formulario.passaporte, formulario.endereco, formulario.saude, formulario.documentoColaborador, usuarioLogado.OrgId);
                return ret;
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;

                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("AutoCompleteColaboradorPorDiretoria")]
        public ActionResult<ListColaboradoresResult> AutoCompleteColaboradorPorDiretoria(string nome, string cpf, string diretoria, int cursor, int limite, string codGestor)
        {
            var ret = new ListColaboradoresResult();
            try
            {
                ret.Colaboradores = _colaboradorService.AutoCompleteColaboradorPorDiretoria(nome, cpf, diretoria, cursor, limite, codGestor, _aspNetUser.GetUsuarioLogado().OrgId);
                return Ok(ret);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;

                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("AutoCompleteColaboradorPorUnidade")]
        public ActionResult<ListColaboradoresResult> AutoCompleteColaboradorPorUnidade(string nome, string unidade, int cursor, int limite, string codGestor)
        {
            var ret = new ListColaboradoresResult();
            try
            {
                ret.Colaboradores = _colaboradorService.AutoCompleteColaboradorPorDiretoria(nome, null, unidade, cursor, limite, codGestor, _aspNetUser.GetUsuarioLogado().OrgId);
                return Ok(ret);
            }
            catch (Exception e)
            {
                _log.Log("EXCEPTION", LevelsEnum.Critical);
                _log.Log(e.StackTrace, LevelsEnum.Critical);
                ret.Sucesso = false;

                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("BuscarHoleritesColaborador")]
        public ActionResult<BuscarHoleritesResult> BuscarHoleritesColaborador()
        {
            try
            {
                string cpfColaborador = _aspNetUser.GetUsuarioLogado().Cpf;

                var retorno = new
                {
                    holeritesPorAno = _colaboradorService.BuscarHolerites(cpfColaborador)
                    .ToList()
                    .GroupBy(x => x.ano)
                    .ToDictionary(
                        grupo => grupo.Key,
                        grupo => grupo.Select(x => new HoleriteSimplesDTO
                        {
                            path = x.path,
                            ano = x.ano,
                            emissao = (DateTime)x.emissao,
                            mes = x.mes
                        }).ToList()
                    ).Select(kvp => new
                    {
                        ano = kvp.Key,
                        holerites = kvp.Value.Select(holerite => new
                        {
                            holerite.path,
                            emissao = holerite.emissao.ToString("dd-MM-yyyy"),
                            holerite.mes,
                            holerite.ano
                        })
                    })
                };

                return Ok(retorno);
            }
            catch (Exception ex)
            {
                return BadRequest(new { sucesso = false, mensagem = "Erro ao buscar holerites", erros = ex.Message });
            }
        }

        [HttpGet("ListaColaboradoresOrgId")]
        public async Task<ActionResult<ApiGenericResult<List<ColaboradoresOrgDTO>>>> ListaColaboradoresOrg(int cursor, int limite, string nomeOuEmail = "", bool fourtalents = false, string codExterno = "")
        {
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();

            var ret = new ApiGenericResult<List<ColaboradoresOrgDTO>>();
            try
            {
                ret.Retorno = await _colaboradorService.ListaColaboradoresOrgAsync(usuarioLogado.OrgId, cursor, limite, fourtalents, nomeOuEmail, codExterno, usuarioLogado.Cpf);
                return ret;
            }
            catch (ArgumentException e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return BadRequest(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [Authorize]
        [HttpGet("RelatorioColaboradoresSkills")]
        public async Task<IActionResult> RelatorioColaboradoresSkills(bool comHardskill)
        {
            var fileResult = new ApiGenericResult<FileContentResult>();

            fileResult = await _colaboradorService.RelatorioColaboradoresSkills(comHardskill);

            if (!fileResult.Sucesso)
            {
                return StatusCode(500, fileResult.Mensagem);
            }

            return File(fileResult.Retorno.FileContents, fileResult.Retorno.ContentType, fileResult.Retorno.FileDownloadName);
        }

        [Authorize]
        [HttpPost("AlterarIdiomaPadrao")]
        public async Task<ActionResult<UsuarioResult>> AlterarIdiomaPadrao(string idioma)
        {
            var ret = new ApiGenericResult();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                ret.Sucesso = await _colaboradorService.AlterarIdiomaPadrao(idioma, usuarioLogado.Cpf, usuarioLogado.OrgId);

                if (ret.Sucesso)
                {
                    ret.Mensagem = "Idioma Padrão alterado com sucesso.";
                    return Ok(ret);
                }

                ret.Mensagem = "Não foi possível atualizar o Idioma Padrão.";
                return BadRequest(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = "Invalid Access Token";
                return StatusCode(401, ret);
            }
        }

        [HttpGet("AniversariantesSemana")]
        public ActionResult<ApiGenericResult<List<AniversariantesSemanaColaboradorDTO>>> AniversariantesSemana([FromQuery] string? codDiretoria)
        {
            var ret = new ApiGenericResult<List<AniversariantesSemanaColaboradorDTO>>();
            try
            {
                ret.Retorno = _colaboradorService.RetornaListaAniversariantesSemana(_aspNetUser.GetUsuarioLogado().OrgId, codDiretoria).Result;
                return ret;
            }
            catch (ArgumentException e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return BadRequest(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("EnviarCvEmMassa")]
        public async Task<ActionResult<ApiGenericResult<bool>>> EnviarCvEmMassa(EnviarCvEmMassaParam param)
        {
            var ret = new ApiGenericResult<bool>();
            try
            {
                ret.Retorno = await _colaboradorService.EnviarCvEmMassa(param);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("BuscarAnalistasResponsaveis")]
        public async Task<ActionResult<ApiGenericResult<List<AnalistaResponsavelDTO>>>> BuscarAnalistasResponsaveis(int orgId)
        {
            var ret = new ApiGenericResult<List<AnalistaResponsavelDTO>>();
            try
            {
                ret.Retorno = await _colaboradorService.BuscarAnalistasResponsaveis(orgId);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("QualificarColaborador")]
        public async Task<ActionResult<ApiGenericResult<bool>>> QualificarColaborador(string codigoInternoColaborador)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                result.Retorno = await _colaboradorService.QualificarColaborador(codigoInternoColaborador, usuarioLogado.Cpf);
                result.Sucesso = true;
                result.Mensagem = "Colaborador qualificado com sucesso!";
                return Ok(result);
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

        [HttpPost("DesqualificarColaborador")]
        public async Task<ActionResult<ApiGenericResult<bool>>> DesqualificarColaborador(string codigoInternoColaborador)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                result.Retorno = await _colaboradorService.DesqualificarColaborador(codigoInternoColaborador, usuarioLogado.Cpf);
                result.Sucesso = true;
                result.Mensagem = "Colaborador desqualificado com sucesso!";
                return Ok(result);
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

        [HttpGet("QualificadosPorMim")]
        public async Task<ActionResult<ApiGenericResult<List<ColaboradorQualificadoDTO>>>> QualificadosPorMim()
        {
            var result = new ApiGenericResult<List<ColaboradorQualificadoDTO>>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                result.Retorno = await _colaboradorService.GetColaboradoresQualificadosPorMim(usuarioLogado.Cpf);
                result.Sucesso = true;
                return Ok(result);
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

        [HttpGet("QualificadosPorColaborador")]
        public async Task<ActionResult<ApiGenericResult<List<ColaboradorQualificadoDTO>>>> QualificadosPorColaborador(string codColaboradorRecrutador)
        {
            var result = new ApiGenericResult<List<ColaboradorQualificadoDTO>>();
            try
            {
                if (string.IsNullOrEmpty(codColaboradorRecrutador))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Código do recrutador é obrigatório";
                    return BadRequest(result);
                }

                result.Retorno = await _colaboradorService.GetColaboradoresQualificadosPorColaborador(codColaboradorRecrutador);
                result.Sucesso = true;
                return Ok(result);
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

        [HttpPost("SalvarDispositivoColaboradorApp")]
        public async Task<ActionResult<ApiGenericResult<bool>>> SalvarDispositivoColaboradorApp(string deviceToken)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                if (String.IsNullOrEmpty(deviceToken))
                {
                    result.Sucesso = false;
                    result.Mensagem = "Device Token NULL ou VAZIO. Enviar um Device Token válido.";
                }

                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                result.Retorno = _colaboradorService.SalvarDispositivoColaboradorApp(usuarioLogado.Cpf, deviceToken, usuarioLogado.OrgId);
                result.Sucesso = true;
                result.Mensagem = "Device token salvo com sucesso!";
                return Ok(result);
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

        [HttpGet("ListarModelosDeContratacaoPorOrg")]
        public async Task<ActionResult<ApiGenericResult<List<ModeloContratacaoDTO>>>> ListarModelosDeContratacaoPorOrg()
        {
            var result = new ApiGenericResult<List<ModeloContratacaoDTO>>();
            try
            {

                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                result.Retorno = await _colaboradorService.ListarModelosDeContratacaoPorOrg(usuarioLogado.OrgId);
                result.Sucesso = true;
                result.Mensagem = "Device token salvo com sucesso!";
                return Ok(result);
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

        [HttpGet("ListarOrigensColaborador")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<OrigemColaboradorDTO>>>> ListarOrigensColaborador()
        {
            var result = new ApiGenericResult<IEnumerable<OrigemColaboradorDTO>>();
            try
            {
                result.Retorno = await _colaboradorService.ListarOrigensColaborador();
                return Ok(result);
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

        [HttpGet("ListarCargos")]
        public async Task<ActionResult<ApiGenericResult<List<CargoColaboradorOrgDTO>>>> ListarCargos()
        {
            var usuarioLogado = _aspNetUser.GetUsuarioLogado();

            var result = await _colaboradorService.ListarCargosPorOrgIdAsync(usuarioLogado.OrgId, usuarioLogado.Cpf);
            return Ok(result);
        }

        [HttpPost("EditarDadosColaborador")]
        public async Task<ActionResult<ApiGenericResult<DataTransferObject.Domain.Colaborador.EditarColaboradorDTO>>> EditarDadosColaborador([FromBody] DataTransferObject.Domain.Colaborador.EditarColaboradorDTO colaborador)
        {
            var result = new ApiGenericResult<DataTransferObject.Domain.Colaborador.EditarColaboradorDTO>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                var colaboradorAtualizado = await _colaboradorService.AtualizarColaboradorAsync(colaborador, usuarioLogado.Cpf);

                result.Retorno = colaboradorAtualizado;
                result.Sucesso = true;
                result.Mensagem = "Dados do colaborador atualizados com sucesso!";

                return Ok(result);
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
                result.Mensagem = "Erro interno ao atualizar dados do colaborador";
                return StatusCode(500, result);
            }
        }

        [HttpGet("ObterDadosColaborador")]
        public async Task<ActionResult<ApiGenericResult<DataTransferObject.Domain.Colaborador.EditarColaboradorDTO>>> ObterDadosColaborador(string codigoInternoColaborador)
        {
            var result = new ApiGenericResult<DataTransferObject.Domain.Colaborador.EditarColaboradorDTO>();
            try
            {
                var colaborador = await _colaboradorService.ObterColaboradorPorCodigoAsync(codigoInternoColaborador);

                if (colaborador == null)
                {
                    result.Sucesso = false;
                    result.Mensagem = "Colaborador não encontrado";
                    return NotFound(result);
                }

                result.Retorno = colaborador;
                result.Sucesso = true;
                result.Mensagem = "Dados do colaborador obtidos com sucesso!";

                return Ok(result);
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
                result.Mensagem = "Erro interno ao obter dados do colaborador";
                return StatusCode(500, result);
            }
        }

        [HttpGet("ListarLogsColaborador")]
        public async Task<ActionResult<ApiGenericResult<List<DataTransferObject.Domain.Colaborador.ColaboradorLogDTO>>>> ListarLogsColaborador(string codigoInternoColaborador, int limite = 10, int cursor = 0)
        {
            var result = new ApiGenericResult<List<DataTransferObject.Domain.Colaborador.ColaboradorLogDTO>>();
            try
            {
                var logs = await _colaboradorService.ListarLogsPorColaboradorAsync(codigoInternoColaborador, limite, cursor);

                result.Retorno = logs;
                result.Sucesso = true;
                result.Mensagem = "Logs do colaborador obtidos com sucesso!";

                return Ok(result);
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
                result.Mensagem = "Erro interno ao obter logs do colaborador";
                return StatusCode(500, result);
            }
        }
    }
}