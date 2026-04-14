using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Competencia.API.DTOs;
using Competencia.Domain.Enums;
using Competencia.Domain.Interfaces.Services;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Certificado;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Foursys;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataTransferObject.Domain.Competencia.MapaCompetencia;

namespace Competencia.API.Controllers
{
    [Authorize]
    [HandleException]
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class CompetenciaController : ControllerBase
    {
        private readonly ICompetenciaService _competenciaService;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public CompetenciaController(IAspNetUser aspNetUser, ICompetenciaService competenciaService, IVerificaSeCpfESistemico verificaCpfSistemico)
        {
            _competenciaService = competenciaService;
            _verificaCpfSistemico = verificaCpfSistemico;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [AllowAnonymous]
        [HttpGet("ListarCompetencia")]
        public ActionResult<ListaCompetenciaResult> ListarCompetencia(string busca, int cursor, int limite)
        {
            var ret = new ListaCompetenciaResult();
            try
            {
                var retService = _competenciaService.ListCompetencia(busca, cursor, limite);

                ret.Competencias = retService.Select(x => new ItemPerfilDTO
                {
                    Descricao = x.Descricao,
                    Id = x.Id,
                    Pendente = x.Pendente
                }).ToList();

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("GetCompetenciaById")]
        public ActionResult<CompetenciaResult> GetCompetenciaById(long id)
        {
            var ret = new CompetenciaResult();
            try
            {
                var retService = _competenciaService.GetCompetenciaById(id);
                if (retService != null)
                {
                    ret.Competencia = new ItemPerfilDTO
                    {
                        Descricao = retService.Descricao,
                        Id = retService.Id,
                        Pendente = retService.Pendente
                    };
                }
                else
                {
                    ret.Competencia = new ItemPerfilDTO
                    {
                        Descricao = null,
                        Id = id,
                    };
                    ret.Sucesso = false;
                    ret.Mensagem = "Competencia não encontrada.";
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

        [HttpGet("ListarCompetenciaColaborador")]
        public async Task<ActionResult> ListarCompetenciaColaborador(string cpfColaborador)
        {
            var ret = new ListaCompetenciaColaboradorResult();
            try
            {
                var retService = await _competenciaService
                    .ListCompetenciaColaborador(cpfColaborador);

                foreach (var item in retService)
                    ret.Competencia.Add(item);

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AdicionarCompetenciaColaborador")]
        public ActionResult AdicionarCompetenciaColaborador(List<AddCompetenciaColabParam> param)
        {
            var ret = new CompetenciaColaboradorResult();
            try
            {
                ret = _competenciaService.AddCompetenciaColaboradorEmLote(param, _usuarioLogado.Cpf);
                return Ok(ret);
            }
            catch (ArgumentException ae)
            {
                ret.Sucesso = false;
                ret.Mensagem = ae.Message;
                return StatusCode(400, ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("RemoverCompetenciaColaborador")]
        public ActionResult<StatusResult> RemoverCompetenciaColaborador(RemoveCompetenciaColabParam param)
        {
            var ret = new StatusResult();
            try
            {
                var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);

                _competenciaService.RemoveCompetenciaColaborador(param.CompetenciaId, cpfRequest);
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

        [HttpGet("ListarNivelCompetencia")]
        public ActionResult<ListaNivelResult> ListarNivelCompetencia()
        {
            var ret = new ListaNivelResult();
            try
            {
                var retService = _competenciaService.ListaNivelCompetencia();

                foreach (var item in retService)
                    ret.Niveis.Add(item);

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }
        
        [RequestSizeLimit(100_000_000)]
        [HttpPost("AdicionarCertificadoCompetenciaColaborador")]
        [HttpPost("InserirCertificadoCompetenciaColaborador")]
        public async Task<ActionResult<CertificadoResult>> InserirCertificadoCompetenciaColaboradorAsync()
        {
            var ret = new CertificadoResult();
            var httpRequest = HttpContext.Request;
            try
            {
                StringValues cpfRequest;
                httpRequest.Form.TryGetValue("cpfRequest", out cpfRequest);

                StringValues competenciaColaboradorId;
                httpRequest.Form.TryGetValue("competenciaColaboradorId", out competenciaColaboradorId);

                StringValues tipo;
                httpRequest.Form.TryGetValue("tipo", out tipo);

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

                var a = httpRequest.Form.Files.FirstOrDefault();

                byte[] imagem = null;

                var filePath = Path.GetTempFileName();
                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
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
                                    case "application/pdf":
                                    case "image/png":
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
                DateTime dataConvert = DateTime.ParseExact(dataConclusao, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                ret.Certificado = await _competenciaService.InserirCertificadoCompetenciaColaborador(cpfRequest, long.Parse(competenciaColaboradorId.FirstOrDefault()), imagem,
                    Enum.Parse<TipoCertificadoEnum>(tipo.FirstOrDefault()), dataConvert, descricao, instituicao, cargaHorariaInt);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("RemoveCertificadoCompetenciaColaborador")]
        public ActionResult<StatusResult> RemoveCertificadoCompetenciaColaborador(long id, string cpf)
        {
            var ret = new StatusResult();
            try
            {
                _competenciaService.RemoveCertificadoCompetenciaColaborador(id, _usuarioLogado.Cpf);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("AlteraCertificadoPrincipalColaborador")]
        public ActionResult<StatusResult> AlteraCertificadoPrincipalColaborador(long id, string cpf)
        {
            var ret = new StatusResult();
            try
            {
                _competenciaService.AlteraCertificadoPrincipalColaborador(id, cpf);

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }
        
        [RequestSizeLimit(100_000_000)]
        [HttpPost("AlterarCertificado")]
        public async Task<ActionResult<CertificadoResult>> AlteraCertificado()
        {
            var ret = new CertificadoResult();

            try
            {
                var httpRequest = HttpContext.Request;
                StringValues cpfRequest;
                httpRequest.Form.TryGetValue("cpfRequest", out cpfRequest);
                StringValues idCertificado;
                httpRequest.Form.TryGetValue("idCertificado", out idCertificado);
                StringValues instituicao;
                httpRequest.Form.TryGetValue("instituicao", out instituicao);
                StringValues dataConclusao;
                httpRequest.Form.TryGetValue("dataConclusao", out dataConclusao);
                StringValues descricao;
                httpRequest.Form.TryGetValue("descricao", out descricao);
                StringValues tipo;
                httpRequest.Form.TryGetValue("tipo", out tipo);
                byte[] imagem = null;

                StringValues cargaHoraria;
                httpRequest.Form.TryGetValue("cargaHoraria", out cargaHoraria);

                int cargaHorariaInt;
                if (!int.TryParse(cargaHoraria, out cargaHorariaInt))
                {
                    throw new Exception("Carga horária deve ser registrada com números initeiros");
                }

                int idCertificadoInt;
                if (!int.TryParse(idCertificado, out idCertificadoInt))
                {
                    throw new Exception("certificado não encontrado");
                }

                var filePath = Path.GetTempFileName();
                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
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
                                    case "application/pdf":
                                    case "image/png":
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

                var enumerador = Enum.Parse<TipoCertificadoEnum>(tipo.ToString());
                DateTime dataConvert = DateTime.ParseExact(dataConclusao, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                if (string.IsNullOrEmpty(tipo))
                {
                    ret.Certificado = await _competenciaService.AlteraCertificado(cpfRequest, idCertificadoInt, imagem,
                    TipoCertificadoEnum.NULL, dataConvert, descricao, instituicao, cargaHorariaInt);
                }
                else
                {
                    ret.Certificado = await _competenciaService.AlteraCertificado(cpfRequest, idCertificadoInt, imagem,
                    enumerador, dataConvert, descricao, instituicao, cargaHorariaInt);
                }

                return (ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AtualizaCompetenciaColaborador")]
        public ItemPerfilResult AtualizaCompetenciaColaborador(AdicionarRemoverItemParam param)
        {
            var ret = new ItemPerfilResult();
            try
            {
                var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);

                try
                {
                    var retservice = _competenciaService.AlterarCompetenciaColaborador(cpfRequest, param.Id, param.NivelId);
                    ret.ItemId = retservice.Id;
                }
                catch (Exception e)
                {
                    ret = new ItemPerfilResult
                    {
                        Sucesso = false,
                        ItemId = param.Id,
                        Mensagem = e.Message
                    };
                }
                ret.Sucesso = true;
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("MergeCompetenciaColaborador")]
        public async Task<StatusResult> MergeCompetenciaColaborador(MergeItemPerfilParam param)
        {
            var ret = new StatusResult();
            try
            {
                var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);

                var lstItensColab = await _competenciaService.ListCompetenciaColaborador(cpfRequest);
                var lstInsert = new List<AdicionarRemoverItemParam>();
                var lstDelete = new List<AdicionarRemoverItemParam>();
                var listItemColabDto = new List<CompetenciaColaboradorDTO>();

                foreach (var item in lstItensColab)
                {
                    listItemColabDto.Add(item);
                }

                lstDelete = listItemColabDto.Where(x => param.Itens.Where(y => y.Id == x.Competencia.Id).Count() == 0).Select(x =>
                {
                    return new AdicionarRemoverItemParam
                    {
                        Id = x.Competencia.Id,
                        NivelId = x.Nivel?.Id,
                        Cpf = cpfRequest
                    };
                }).ToList();

                lstInsert = param.Itens.Where(x => listItemColabDto.Where(y => y.Competencia.Id == x.Id).Count() == 0).Select(x =>
                {
                    return new AdicionarRemoverItemParam
                    {
                        Id = x.Id,
                        NivelId = null,
                        Cpf = cpfRequest
                    };
                }).ToList();

                foreach (var item in lstInsert)
                {
                    try
                    {
                        _competenciaService.AddCompetenciaColaborador(item.Id, item.NivelId, cpfRequest);
                    }
                    catch { /*Ignora*/ }
                }

                foreach (var item in lstDelete)
                {
                    try
                    {
                        _competenciaService.RemoveCompetenciaColaborador(item.Id, cpfRequest);
                    }
                    catch { /*Ignora*/ }
                }

                ret.Sucesso = true;
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("ListarIdsPorCompetenciaId")]
        public ActionResult<ListaIdsPorCompetenciaResult> ListarIdsPorCompetenciaId(long id)
        {
            var ret = new ListaIdsPorCompetenciaResult();
            try
            {
                ret.ListaDeIds = _competenciaService.ListarIdsPorCompetenciaId(id);

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("RemoverCertificado")]
        public ActionResult<StatusResult> RemoverCertificado(RemoveCertificadoParam param)
        {
            var ret = new StatusResult();
            try
            {
                _competenciaService.RemoverCertificado(param.certificadoId, _usuarioLogado.Cpf);
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

        [HttpPost("SumarioCompetenciasListarColaboradores")]
        public ActionResult<CompetenciasSumarioResult> SumarioCompetenciasListarColaboradores(FiltroSumarioCompetenciasParam param)
        {
            var ret = new CompetenciasSumarioResult();
            try
            {
                ret = _competenciaService.SumarioCompetencias(param);
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                Console.WriteLine("StackTrace: \n" + e.StackTrace);
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("SumarioCompetenciasListarSkills")]
        public async Task<ActionResult<ItensSumarioResult>> SumarioCompetenciasListarSkills(int cursor, int limite, string? descricao)
        {
            var ret = new ItensSumarioResult();
            try
            {
                ret = await _competenciaService.SumarioCompetenciasListarSkills(cursor, limite, descricao);
                ret.Sucesso = true;

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("SumarioCompetenciasListarNiveis")]
        public ActionResult SumarioCompetenciasListarNiveis()
        {
            var ret = new ApiGenericResult<List<SkillSumarioDTO>>();
            try
            {
                ret.Retorno = _competenciaService.SumarioCompetenciasListarNiveis();
                ret.Sucesso = true;

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [AllowAnonymous]
        [HttpGet("ListarCompetenciaNaoAtribuidas")]
        public ActionResult<ListaCompetenciaResult> ListarCompetenciaNaoAtribuidas(string busca, int cursor, int limite)
        {
            var ret = new ListaCompetenciaResult();
            try
            {
                var listaIds = _competenciaService.ListarCompetenciasAtribuidas(_usuarioLogado.Cpf);

                var retService = _competenciaService.ListCompetencia(busca, cursor, limite);

                if (listaIds.Count > 0)
                {
                    retService = retService.Where(x => !listaIds.Contains(x.Id)).ToList();
                }

                ret.Competencias = retService.Select(x => new ItemPerfilDTO
                {
                    Descricao = x.Descricao,
                    Id = x.Id,
                    Pendente = x.Pendente
                }).ToList();

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarUnidadesComDefaultPorOrgId")]
        public ListaUnidadesResult ListarUnidadesComDefaultPorOrgId()
        {
            var ret = new ListaUnidadesResult();
            try
            {
                ret.ListaUnidades = _competenciaService.ListUnidadesComDefaultPorOrg(_usuarioLogado.Token, _usuarioLogado.OrgId);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }

        [HttpGet("ListarUnidadesPorOrgId")]
        public ListaUnidadesResult ListarUnidadesPorOrgId()
        {
            var ret = new ListaUnidadesResult();
            try
            {
                ret.ListaUnidades = _competenciaService.ListUnidadesPorOrg(_usuarioLogado.Token, _usuarioLogado.OrgId);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;

                return ret;
            }
        }
        [HttpGet("ListarCompetenciasSugeridas")]
        public async Task<ActionResult> ListarCompetenciasSugeridas(TipoCompetenciaSRSEnum enumTipoCompetencia)
        {
            var ret = new ApiGenericResult<List<CompetenciaSugeridaDTO>>();
            try
            {
                ret.Retorno = _competenciaService.ListarCompetenciasSugeridas(enumTipoCompetencia);
                return Ok(ret);
            }
            catch (UnauthorizedAccessException e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(401, ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message.ToString();
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AprovarCompetencia")]
        public async Task<ActionResult> AprovarCompetencia(int idCompetenciaASerAprovada, TipoCompetenciaSRSEnum enumTipoCompetencia)
        {
            var ret = new StatusResult();
            try
            {
                await _competenciaService.AprovarCompetencia(idCompetenciaASerAprovada, enumTipoCompetencia);

                return Ok(ret);
            }
            catch (UnauthorizedAccessException e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(401, ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message.ToString();
                return StatusCode(500, ret);
            }
        }

        [HttpPost("ReprovarCompetencia")]
        public async Task<ActionResult> ReprovarCompetencia([FromBody] ReprovarCompetenciaParam param)
        {
            var ret = new ApiGenericResult<string>();
            try
            {
                await _competenciaService.ReprovarCompetencia(param.Id, param.TipoCompetencia);
                return Ok(ret);
            }
            catch (UnauthorizedAccessException e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(401, ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message.ToString();
                return StatusCode(500, ret);
            }
        }

        [HttpPost("UnificarCompetencia")]
        public async Task<ActionResult> UnificarCompetencia([FromBody] UnificarCompetenciaParam param)
        {
            var ret = new ApiGenericResult<string>();
            await _competenciaService.UnificarCompetencia(param.IdCompetencia, param.IdCompetenciaConsolidada, param.TipoCompetenciaEnum, _usuarioLogado.Cpf, _usuarioLogado.Token);
            return Ok(ret);
        }

        [HttpGet("ListarCompetenciasConsolidadas")]
        public async Task<ActionResult> ListarCompetenciasConsolidadas(TipoCompetenciaSRSEnum enumTipoCompetencia)
        {
            var ret = new ApiGenericResult<List<CompetenciaConsolidadaDTO>>();
            try
            {
                ret.Retorno = await _competenciaService.ListarCompetenciasConsolidadas(enumTipoCompetencia);

                return Ok(ret);
            }
            catch (UnauthorizedAccessException e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(401, ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message.ToString();
                return StatusCode(500, ret);
            }
        }
        [HttpGet("ListarLogCompetencias")]
        public async Task<ActionResult> ListarLogCompetencias(TipoCompetenciaSRSEnum enumTipoCompetencia)
        {
            var ret = new ApiGenericResult<List<LogCompetenciaDTO>>();

            ret.Retorno = await _competenciaService.ListarLogCompetencias(enumTipoCompetencia);

            return Ok(ret);
        }
        [HttpPost("EditarCompetencia")]
        public async Task<ActionResult> EditarCompetencia([FromBody] EditarCompetenciaParam param)
        {
            var ret = new ApiGenericResult<EditarCompetenciaDTO>();
            ret.Retorno = await _competenciaService.EditarCompetencia(param);
            return Ok(ret);
        }

        [HttpPost("AdicionarCompetencia")]
        public async Task<ActionResult> AdicionarCompetencia([FromBody] AdicionarCompParam param)
        {
            var ret = new ApiGenericResult<AdicionarCompetenciaDTO>();

            ret.Retorno = await _competenciaService.AdicionarCompetencia(param.Descricao, param.TipoCompetenciaEnum, _usuarioLogado.Cpf);
            return Ok(ret);
        }

        [HttpGet("ListarPerfilContratacao")]
        public ActionResult<ApiGenericResult<List<PerfilAlocacaoDTO>>> ListarPerfilContratacao()
        {
            var ret = new ApiGenericResult<List<PerfilAlocacaoDTO>>
            {
                Retorno = _competenciaService.ListarPerfilAlocacao(_usuarioLogado.OrgId)
            };
            return Ok(ret);
        }

        [HttpPost("AlterarNomeCompetencia")]
        public async Task<ActionResult<ApiGenericResult<int>>> AlterarNomeCompetencia([FromBody] EditarNomeCompetenciaParam param)
        {
            var ret = await _competenciaService.AlterarNomeCompetencia(param.NomeCompetencia, param.NovoNomeCompetencia,
                param.TipoCompetencia, _usuarioLogado.Cpf, _usuarioLogado.Token);
            return Ok(ret);
        }
        
        [HttpGet("ListarNomeDeSkillsPorTipo")]
        public async Task<ActionResult<ApiGenericResult<List<CompetenciaNomeEIdDTO>>>> ListarNomeDeSkillsPorTipo([FromQuery] TipoCompetenciaSRSEnum tipo)
        {
            var ret = await _competenciaService.ListarNomeDeSkillsPorTipo(tipo);
            return Ok(ret);
        }
        
        [AllowAnonymous]
        [HttpGet("SincronizarSkillsNaoUnificadasNaCuradoriaAntiga")]
        public async Task<ActionResult<ApiGenericResult<List<CompetenciaNomeEIdDTO>>>> SincronizarSkillsNaoUnificadasNaCuradoriaAntiga()
        {
            await _competenciaService.SincronizarSkillsNaoUnificadasNaCuradoriaAntiga();
            return Ok();
        }

        [HttpGet("ListarTodasSkillsColaborador")]
        public async Task<ActionResult<ApiGenericResult<List<VwSkillColaboradorDTO>>>> ListarTodasSkillsColaborador([FromQuery]string codigoInternoColaborador)
        {
            var result = await _competenciaService.ListarSkillsColaborador(codigoInternoColaborador);
            return Ok(result);
        }

        [HttpPost("GravarLogsSkillsMinhaJornada")]
        public async Task<ActionResult<ApiGenericResult<SkillsLog>>> GravarLogsSkillsMinhaJornada([FromBody] SkillsLog logSkill)
        {
            var result = await _competenciaService.GravarLogsSkillsMinhaJornada(logSkill);
            return Ok(result);
        }
    }
}