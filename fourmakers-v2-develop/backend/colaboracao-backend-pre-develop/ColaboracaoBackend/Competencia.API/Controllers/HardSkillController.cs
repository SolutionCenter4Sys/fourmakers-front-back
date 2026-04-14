using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Competencia.API.DTOs;
using Competencia.Domain.Interfaces.Services;
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

namespace Competencia.API.Controllers
{
    [Authorize]
    [Route("api/Competencia/[controller]")]
    [ApiController]
    [LogAction]
    [HandleException]
    public class HardSkillController : ControllerBase
    {
        private readonly ICompetenciaService _competenciaService;
        private readonly IHardSkillService _hardSkillService;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public HardSkillController(IAspNetUser aspNetUser, IHardSkillService hardSkillService, IVerificaSeCpfESistemico verificaCpfSistemico)
        {
            _hardSkillService = hardSkillService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
            _verificaCpfSistemico = verificaCpfSistemico;
        }

        [HttpPost("AdicionarHardSkillColaborador")]
        public ActionResult AdicionarHardSkillColaborador([FromBody]List<AddCompetenciaColabParam> param, [FromQuery] bool minhaJornada = false)
        {
            var ret = new CompetenciaColaboradorResult();
            ret = _hardSkillService.InserirHardSkillColaboradorEmLote(param, _usuarioLogado.Cpf, minhaJornada);
            return Ok(ret);
        }

        [AllowAnonymous]
        [HttpGet("ListarCompetencia")]
        public ActionResult<ListaCompetenciaResult> ListarCompetencia(string busca, int cursor, int limite)
        {
            var ret = new ListaCompetenciaResult();

            ret.Competencias = _hardSkillService.ListarHardSkill(busca, cursor, limite);
            ret.Nivel = _hardSkillService.ListarNivelHardSkill();

            return Ok(ret);
        }

        [HttpGet("GetCompetenciaById")]
        public ActionResult<CompetenciaResult> GetCompetenciaById(long id)
        {
            var ret = new CompetenciaResult();
            var retService = _hardSkillService.ObterHardSkillPorId(id);
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

        [HttpPost("AdicionarCompetencia")]
        public ActionResult<CompetenciaResult> AdicionarCompetencia(AddCompetenciaParam param)
        {
            var ret = new CompetenciaResult();
            var retService = _hardSkillService.InserirHardSkill(param.descricao.ToUpper(), _usuarioLogado.Cpf);

            ret.Competencia = new ItemPerfilDTO()
            {
                Descricao = retService.Descricao,
                Id = retService.Id
            };
            return Ok(ret);
        }

        [HttpGet("GetGroupCompetenciaById")]
        public async Task<ActionResult<List<CompetenciaGroupDTO>>> GetGroupCompetenciaByIds(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return BadRequest("Informe ao menos um id.");

            // separa por vírgula → converte para long
            var listaIds = ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.Parse(x.Trim()))
                .ToList();

            var competencias = await _hardSkillService.GetGroupCompetenciaByIds(listaIds);

            return Ok(competencias);
        }

        [HttpGet("ListarCompetenciaColaborador")]
        public async Task<ActionResult> ListarCompetenciaColaborador(string cpfColaborador)
        {
            var ret = new ListaCompetenciaColaboradorResult();
            var retService = _hardSkillService
                .ListarHardSkillColaboradorCompleto(cpfColaborador);

            ret.Competencia = retService;
            return Ok(ret);
        }
        [HttpPost("GetHardSkillInfoByDescricao")]
        public async Task<ActionResult> GetHardSkillInfoByDescricao(GetHardSkillInfoByDescricao param)
        {
            try
            {
                return Ok(await _hardSkillService.GetHardSkillInfoByDescricaoAsync(param.Skills));
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message + "\nStackTrace: " + err.StackTrace);
                return StatusCode(500, "Erro interno servidor");
            }
        }

        [HttpPost("RemoverCompetenciaColaborador")]
        public ActionResult<StatusResult> RemoverCompetenciaColaborador(RemoveCompetenciaColabParam param)
        {
            var ret = new StatusResult();
            var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);
            ret.Sucesso = _hardSkillService.RemoverHardSkillColaborador(param.CompetenciaId, cpfRequest);
            return Ok(ret);
        }

        [HttpGet("ListarNivelCompetencia")]
        public ActionResult<ListaNivelResult> ListarNivelCompetencia()
        {
            var ret = new ListaNivelResult();
            ret.Niveis = _hardSkillService.ListarNivelHardSkill();
            return Ok(ret);
        }

        [HttpGet("ListarCertificadoColaborador")]
        public ActionResult ListarCertificadoColaborador()
        {
            var ret = new CertificadoColaboradorResult();
            var retService = _hardSkillService
                .ListaCertificadoColaborador(_usuarioLogado.Cpf);

            ret.Certificados = retService;
            return Ok(ret);
        }

        [HttpGet("ListarCertificadoColaboradorPorCodigoInterno")]
        public ActionResult ListarCertificadoColaboradorPorCodigoInterno(string codInternoColaborador)
        {
            var ret = new CertificadoColaboradorResult();
            var retService = _hardSkillService
                .ListaCertificadoColaboradorPorCodigoInterno(codInternoColaborador, _usuarioLogado.Token);

            ret.Certificados = retService;
            return Ok(ret);
        }

        [HttpPost("InserirCertificadoCompetenciaColaborador")]
        public async Task<ActionResult<CertificadoResult>> InserirCertificadoCompetenciaColaboradorAsync()
        {
            var ret = new CertificadoResult();
            var httpRequest = HttpContext.Request;

            httpRequest.Form.TryGetValue("cpfRequest", out StringValues cpfRequest);

            httpRequest.Form.TryGetValue("competenciaColaboradorId", out StringValues competenciaColaboradorId);

            httpRequest.Form.TryGetValue("tipo", out StringValues tipo);

            httpRequest.Form.TryGetValue("instituicao", out StringValues instituicao);

            httpRequest.Form.TryGetValue("dataConclusao", out StringValues dataConclusao);

            httpRequest.Form.TryGetValue("descricao", out StringValues descricao);

            httpRequest.Form.TryGetValue("cargaHoraria", out StringValues cargaHoraria);

            if (!int.TryParse(cargaHoraria, out int cargaHorariaInt))
            {
                throw new Exception("Carga horária deve ser registrada com números inteiros.");
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

            ret.Certificado = await _hardSkillService.InserirCertificadoHardSkillColaborador(_usuarioLogado.Cpf, long.Parse(competenciaColaboradorId.FirstOrDefault()), imagem,
                Enum.Parse<TipoCertificadoEnum>(tipo.FirstOrDefault()), dataConvert, descricao, instituicao, cargaHorariaInt);

            return ret;
        }

        [HttpPost("RemoveCertificadoCompetenciaColaborador")]
        public ActionResult<StatusResult> RemoveCertificadoCompetenciaColaborador(long id, string cpf)
        {
            var ret = new StatusResult();
            _hardSkillService.RemoverCertificadoHardSkillColaborador(id, _usuarioLogado.Cpf);
            return ret;
        }

        [HttpPost("AlteraCertificadoPrincipalColaborador")]
        public ActionResult<StatusResult> AlteraCertificadoPrincipalColaborador(long id, string cpf)
        {
            var ret = new StatusResult();
            _hardSkillService.AlterarCertificadoPrincipalColaborador(id, cpf);
            return ret;
        }

        [HttpPost("AlterarCertificado")]
        public async Task<CertificadoResult> AlteraCertificado()
        {
            var ret = new CertificadoResult();
            var httpRequest = HttpContext.Request;
            httpRequest.Form.TryGetValue("cpfRequest", out StringValues cpfRequest);
            httpRequest.Form.TryGetValue("idCertificado", out StringValues idCertificado);
            httpRequest.Form.TryGetValue("instituicao", out StringValues instituicao);
            httpRequest.Form.TryGetValue("dataConclusao", out StringValues dataConclusao);
            httpRequest.Form.TryGetValue("descricao", out StringValues descricao);
            httpRequest.Form.TryGetValue("tipo", out StringValues tipo);
            byte[] imagem = null;

            httpRequest.Form.TryGetValue("cargaHoraria", out StringValues cargaHoraria);

            if (!int.TryParse(cargaHoraria, out int cargaHorariaInt))
            {
                throw new Exception("Carga horária deve ser registrada com números initeiros");
            }

            if (!int.TryParse(idCertificado, out int idCertificadoInt))
            {
                throw new Exception("certificado não encontrado");
            }

            var filePath = Path.GetTempFileName();
            foreach (var formFile in Request.Form.Files)
            {
                if (formFile.Length > 0)
                {
                    using var inputStream = new FileStream(filePath, FileMode.Create);
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

            var enumerador = Enum.Parse<TipoCertificadoEnum>(tipo.ToString());
            DateTime dataConvert = DateTime.ParseExact(dataConclusao, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(tipo))
            {
                ret.Certificado = await _hardSkillService.AlterarCertificado(cpfRequest, idCertificadoInt, imagem,
                TipoCertificadoEnum.NULL, dataConvert, descricao, instituicao, cargaHorariaInt);
            }
            else
            {
                ret.Certificado = await _hardSkillService.AlterarCertificado(cpfRequest, idCertificadoInt, imagem,
                enumerador, dataConvert, descricao, instituicao, cargaHorariaInt);
            }

            return (ret);
        }

        [HttpPost("AtualizaCompetenciaColaborador")]
        public ItemPerfilResult AtualizaCompetenciaColaborador([FromBody] AdicionarRemoverItemParam param, [FromQuery] bool minhaJornada)
        {
            var ret = new ItemPerfilResult();
            var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);

            try
            {
                var retservice = _hardSkillService.AlterarHardSkillColaborador(cpfRequest, param.Id, param.NivelId, minhaJornada, param.GestorExternoPerfil, _usuarioLogado.Cpf);
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

        [HttpGet("ListarIdsPorCompetenciaId")]
        public ActionResult<ListaIdsPorCompetenciaResult> ListarIdsPorCompetenciaId(long id)
        {
            var ret = new ListaIdsPorCompetenciaResult();
            ret.ListaDeIds = _hardSkillService.ListarIdsPorHardSkillId(id);
            return Ok(ret);
        }

        [HttpPost("RemoverCertificado")]
        public ActionResult<StatusResult> RemoverCertificado(RemoveCertificadoParam param)
        {
            var ret = new StatusResult();
            _hardSkillService.RemoverCertificado(param.certificadoId, _usuarioLogado.Cpf);
            ret.Sucesso = true;
            return Ok(ret);
        }

        [HttpPost("SumarioCompetenciasListarColaboradores")]
        public ActionResult<CompetenciasSumarioResult> SumarioCompetenciasListarColaboradores(FiltroSumarioCompetenciasParam param)
        {
            var ret = new CompetenciasSumarioResult();
            ret = _competenciaService.SumarioCompetencias(param);
            ret.Sucesso = true;
            return Ok(ret);
        }

        [HttpGet("SumarioCompetenciasListarSkills")]
        public ActionResult<ItensSumarioResult> SumarioCompetenciasListarSkills()
        {
            var ret = new ItensSumarioResult();
            ret = _hardSkillService.SumarioCompetenciasListarSkills();
            ret.Sucesso = true;
            return Ok(ret);
        }

        [HttpGet("ListarCompetenciaNaoAtribuidas")]
        public ActionResult<ListaCompetenciaResult> ListarCompetenciaNaoAtribuidas(string busca, int cursor, int limite)
        {
            var ret = _hardSkillService.ListarCompetenciaNaoAtribuidas(busca, cursor, limite, _usuarioLogado.Cpf);
            return Ok(ret);
        }

        [HttpGet("ListarUnidadesComDefaultPorOrgId")]
        public ListaUnidadesResult ListarUnidadesComDefaultPorOrgId()
        {
            var ret = new ListaUnidadesResult();
            ret.ListaUnidades = _hardSkillService.ListUnidadesComDefaultPorOrg(_usuarioLogado.Token, _usuarioLogado.OrgId);
            return ret;
        }

        [HttpGet("ListarUnidadesPorOrgId")]
        public ListaUnidadesResult ListarUnidadesPorOrgId()
        {
            var ret = new ListaUnidadesResult();
            ret.ListaUnidades = _hardSkillService.ListUnidadesPorOrg(_usuarioLogado.Token, _usuarioLogado.OrgId);
            return ret;
        }
    }
}