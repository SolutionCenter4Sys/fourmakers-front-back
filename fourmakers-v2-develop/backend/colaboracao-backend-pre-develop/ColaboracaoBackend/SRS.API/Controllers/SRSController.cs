using Colaboracao.Core.Exceptions;
using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.SRS;
using DataTransferObject.Domain.SRS.Candidate;
using DataTransferObject.Domain.SRS.Offboarding;
using DataTransferObject.Domain.SRS.Onboarding;
using DataTransferObject.Domain.SRS.Tecnica;
using DataTransferObject.Domain.SRS.Vagas;
using DataTransferObject.Domain.Usuario;
using DataTransferObject.Domain.Util.Enum;
using DataTransferObject.Domain.VagasSRS;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SRS.Domain.Impl.Service;
using SRS.Domain.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SRS.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class SRSController : ControllerBase
    {
        private readonly ISRSService _sRSService;
        private readonly IAspNetUser _aspNetUser;
        private readonly ISRSCandidateService _candidateService;
        private readonly IVagaService _vagaService;

        public SRSController(ISRSService sRSService, IAspNetUser aspNetUser, ISRSCandidateService candidateService, IVagaService vagaService)
        {
            _sRSService = sRSService;
            _aspNetUser = aspNetUser;
            _candidateService = candidateService;
            _vagaService = vagaService;
        }

        [AllowAnonymous]
        [HttpGet("ListarVagas")]
        public async Task<ActionResult<SRSResult>> ListarVagas(string busca, int cursor, int limite, FiltroStatusPublicacaoEnum filtroStatusPublicacao, string org)
        {
            var ret = new SRSResult();
            try
            {
                // Primeiro verifica se o usuário está logado (rota autenticada)
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();
                int? orgId = usuarioLogado?.OrgId;
                
                // Se não houver usuário logado, o parâmetro 'org' é obrigatório
                if (usuarioLogado == null)
                {
                    if (string.IsNullOrWhiteSpace(org))
                    {
                        ret.Sucesso = false;
                        ret.Mensagem = "O parâmetro 'org' é obrigatório quando não há usuário logado.";
                        return BadRequest(ret);
                    }
                    
                    var orgIdFromParam = MapOrgToOrgId(org);
                    if (!orgIdFromParam.HasValue)
                    {
                        ret.Sucesso = false;
                        ret.Mensagem = $"Organização '{org}' não encontrada.";
                        return BadRequest(ret);
                    }
                    
                    orgId = orgIdFromParam.Value;
                }
                
                var vagasRecrutamento = await _vagaService.ListarVagasPipeline(cursor, limite, busca, null, null, null, orgId);

                foreach (var vagaRecrutamento in vagasRecrutamento.Value.Retorno)
                {
                    var skills = new List<SkillsVagasDTO>();

                    foreach (var skill in vagaRecrutamento.Skills)
                    {
                        var typeSkillsDescription = "HardSkill";

                        if (skill.TypeSkillsDescription == "COMPETENCIA")
                            typeSkillsDescription = "HardSkill";

                        if (skill.TypeSkillsDescription == "SOFTSKILL")
                            typeSkillsDescription = "Softskill";

                        if (skill.TypeSkillsDescription == "IDIOMA")
                            typeSkillsDescription = "Idioma";

                        skills.Add(new SkillsVagasDTO { SkillDescription = skill.SkillDescription, SkillId = skill.SkillId.ToInt(), SkillNivelDescription = skill.SkillNivelDescription, SkillNivelId = skill.SkillNivelId.ToInt(), TypeSkills = skill.TipoSkillId, TypeSkillsDescription = typeSkillsDescription });
                    }

                    ret.SrsDTO.Add(new SRSDTO
                    { 
                        Titulo = vagaRecrutamento.Titulo,
                        Data_criacao = vagaRecrutamento.Criacao,
                        Vagas_abertas = vagaRecrutamento.Posicoes,
                        Descricao = vagaRecrutamento.Descricao,
                        Status_vaga = "Pipeline",
                        SkillsVagasDTO = skills,
                        Modalidade = vagaRecrutamento.ModalidadeDescricao,
                        Estado = vagaRecrutamento.Estado,
                        NovaEstruturaRecrutamento = true,
                        CodigoVagaRecrutamento = vagaRecrutamento.Codigo,
                        Data_abertura = vagaRecrutamento.Criacao,
                        Cargo = vagaRecrutamento.Cargo,
                        Pais = "Brasil",
                        Cidade = vagaRecrutamento.Cidade,
                        Tracking = vagaRecrutamento.Tracking,
                        OrgId = vagaRecrutamento.OrgId
                    });
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

        [HttpGet("ListarVagasIndicadas")]
        public VagasIndicadasResult ListarVagasIndicadas()
        {
            var ret = new VagasIndicadasResult();
            try
            {
                ret = _sRSService.ListarVagasIndicadas();
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("ListarMotivoDescandidatura")]
        public async Task<SRSResult> ListarMotivoDescandidatura()
        {
            var ret = new SRSResult();
            try
            {
                return await _sRSService.BuscarRazao();
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [AllowAnonymous]
        [HttpGet("DetalharVaga")]
        public async Task<ActionResult<DetalheResult>> DetalharVaga(long id_vaga, long cod_vaga_recrutamento)
        {
            var ret = new DetalheResult();
            try
            {
                if(cod_vaga_recrutamento > 0)
                {
                    var vagaRecrutamento = await _vagaService.ObterVagaRecrutamentoPorCodigoAnonymous(cod_vaga_recrutamento.ToInt());

                    var detalheDTO = new DetalheDTO
                    {
                        Id_Vaga = vagaRecrutamento.Codigo,
                        Titulo = vagaRecrutamento.Titulo,
                        Descricao = vagaRecrutamento.Descricao,
                        Estado = vagaRecrutamento.Estado,
                        Data_abertura = vagaRecrutamento.DataCriacao,
                        SkillsVagasDTO = new List<SkillsVagasDTO>(),
                        Pais = "Brasil",
                        Cidade = vagaRecrutamento.Cidade
                    };

                    if (vagaRecrutamento.Skills != null && vagaRecrutamento.Skills.Any())
                    {
                        foreach (var skill in vagaRecrutamento.Skills)
                        {
                            var typeSkillsDescription = "HardSkill";

                            if (skill.TypeSkillsDescription == "COMPETENCIA")
                                typeSkillsDescription = "HardSkill";

                            if (skill.TypeSkillsDescription == "SOFTSKILL")
                                typeSkillsDescription = "Softskill";

                            if (skill.TypeSkillsDescription == "IDIOMA")
                                typeSkillsDescription = "Idioma";

                            detalheDTO.SkillsVagasDTO.Add(new SkillsVagasDTO
                            {
                                SkillId = skill.SkillId.ToInt(),
                                SkillDescription = skill.SkillDescription,
                                SkillNivelId = skill.SkillNivelId.ToInt(),
                                SkillNivelDescription = skill.SkillNivelDescription,
                                TypeSkills = skill.TipoSkillId,
                                TypeSkillsDescription = typeSkillsDescription,
                                Id = vagaRecrutamento.Codigo
                            });
                        }
                    }

                    ret.Detalhe.Add(detalheDTO);

                    return Ok(ret);
                }

                var retService = _sRSService.BuscarVagaDetalhada(id_vaga);

                // Extrai os IDs das vagas detalhadas
                var idsVagasDetalhadas = retService.Detalhe.Select(item => item.Id_Vaga).ToList();

                // Chama BuscarSkillsVagas com a lista de IDs
                var skillsVagasResult = _sRSService.BuscarSkillsVagas(idsVagasDetalhadas);

                foreach (var item in retService.Detalhe)
                {
                    // Filtra as skills correspondentes à vaga detalhada atual
                    item.SkillsVagasDTO = skillsVagasResult.Skills
                        .Where(skill => skill.Id == item.Id_Vaga)
                        .ToList();
                    ret.Detalhe.Add(item);
                }

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("CandidatoInscritoNasVagas")]
        public async Task<InscricoesCandidatoResult> CandidatoInscritoNasVagas()
        {
            var ret = new InscricoesCandidatoResult();

            try
            {
                var vagasRecrutamento = await _vagaService.ListarCandidaturasPorCodCandidato(_aspNetUser.GetUsuarioLogado().Cpf);

                foreach (var vagaRecrutamento in vagasRecrutamento)
                {
                    var vaga = new VagasInscritoDTO
                    {
                        Jo_Stvaga = vagaRecrutamento.StatusCandidaturaDescricao,
                        Joborder_Title = vagaRecrutamento.Titulo,
                        Status = vagaRecrutamento.StatusCandidaturaId.ToInt(),
                        Date_Created = vagaRecrutamento.DataCriacao.Value,
                        NovaEstruturaRecrutamento = true,
                        CodigoVagaRecrutamento = vagaRecrutamento.Codigo,
                    };

                    ret.Data.Add(vaga);
                }

                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("CandidatarSe")]
        public async Task<ActionResult<ApiGenericResult<CandidatarResult>>> CandidatarSe(CandidatarSeParam param)
        {
            var ret = new ApiGenericResult<CandidatarResult>();
            try
            {
                ret.Retorno = await _sRSService.CandidatarSe(param.joborder_id, param.convite);
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return BadRequest(ret);
            }
        }

        [HttpPost("FavoritarVagas")]
        public FavoritarVagasResult FavoritarVagas([System.ComponentModel.DataAnnotations.Required] long id_vaga, [System.ComponentModel.DataAnnotations.Required] long tb_usuario_id)
        {
            var ret = new FavoritarVagasResult();
            try
            {
                var retservice = _sRSService.FavoritarVagas(id_vaga, tb_usuario_id);
                ret = retservice;
                ret.Sucess = true;
                ret.Message = "Vaga favoritada com sucesso";
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucess = false;
                ret.Message = e.Message;
                return ret;
            }
        }

        [HttpPost("DesfavoritarVagas")]
        public DesfavoritarResult DesfavoritarVagas([System.ComponentModel.DataAnnotations.Required] long id_vaga, [System.ComponentModel.DataAnnotations.Required] long tb_usuario_id)
        {
            var ret = new DesfavoritarResult();
            try
            {
                var retservice = _sRSService.DesfavoritarVagas(id_vaga, tb_usuario_id);
                ret = retservice;
                ret.Sucess = true;
                ret.Message = "Vaga desfavoritada com sucesso";
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucess = false;
                ret.Message = e.Message;
                return ret;
            }
        }

        [HttpGet("ListarVagasFavoritadas")]
        public ActionResult<FavoritarVagasResult> ListarVagasFavoritadas(long tb_usuario_id)
        {
            var ret = new FavoritarVagasResult();
            try
            {
                var retService = _sRSService.ListarVagasFavoritadas(tb_usuario_id);

                ret.VagasFavoritadas.AddRange(retService.VagasFavoritadas);
                ret.Sucess = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucess = false;
                ret.Message = e.Message;
                return ret;
            }
        }

        [HttpPost("DescandidatarSe")]
        public async Task<DescandidatarResult> DescandidatarSe(DescandidatarSeParam param) //Novo formato sem factory

        {
            var ret = new DescandidatarResult();
            try
            {
                var retservice = await _sRSService.DescandidatarSe(param.joborder_id, param.status_reason_cancellation, param.reason_cancellation);
                ret = retservice;
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }
        [HttpPost("GerarConviteVaga")]
        public ActionResult<GerarConviteResult> GerarConviteDaVaga(long joborder_id)
        {
            var ret = new GerarConviteResult();
            try
            {
                ret.Convite = _sRSService.GerarConvite(joborder_id, 0, null, null, null, null).Result;
                ret.Sucesso = true;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("ListarVagasRecomendadas")]
        public IActionResult ListarVagasRecomendadas()
        {
            var ret = new List<RecomendarVagasDTO>();
            var usuarioLogadoCpf = _aspNetUser.GetUsuarioLogado().Cpf;
            var vagasRecomendadas = _sRSService.ListarVagasRecomendadas(usuarioLogadoCpf);

            // Extrai os IDs das vagas recomendadas
            var idsVagasRecomendadas = vagasRecomendadas.Select(vaga => vaga.VagasSrsId).ToList();

            // Chama BuscarSkillsVagas com a lista de IDs
            var skillsVagasResult = _sRSService.BuscarSkillsVagas(idsVagasRecomendadas);

            foreach (var vaga in vagasRecomendadas)
            {
                // Filtra as skills correspondentes à vaga atual
                vaga.Vaga.SkillsVagasDTO = skillsVagasResult.Skills
                    .Where(skill => skill.Id == vaga.VagasSrsId)
                    .ToList();
                ret.Add(vaga);
            }

            return Ok(ret);
        }

        [HttpGet("buscarQuantidadeVagas")]
        public IActionResult BuscarQuantidadeVagas()
        {
            long quantidade = _sRSService.BuscaQuantidadeVaga();
            return Ok(quantidade);
        }

        [HttpPost("IndicarVaga")]
        public async Task<ActionResult<IndicarVagaPremiadaResult>> IndicarVaga()
        {
            IndicarVagaPremiadaResult ret = new IndicarVagaPremiadaResult();
            var httpRequest = HttpContext.Request;

            try
            {
                StringValues param;
                httpRequest.Form.TryGetValue("param", out param);
                var vagaInfo = JsonSerializer.Deserialize<IndicarVagaParam>(param[0]);

                byte[] curriculo = null;
                TipoCertificadoEnum tipo = TipoCertificadoEnum.IMAGEM;

                var filePath = Path.GetTempFileName();
                foreach (var formFile in Request.Form.Files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var inputStream = new FileStream(filePath, FileMode.Create))
                        {
                            if (formFile.Name == "curriculo")
                            {
                                switch (formFile.ContentType)
                                {
                                    case "application/pdf":
                                        await formFile.CopyToAsync(inputStream);
                                        curriculo = new byte[inputStream.Length];
                                        inputStream.Seek(0, SeekOrigin.Begin);
                                        inputStream.Read(curriculo, 0, curriculo.Length);
                                        tipo = TipoCertificadoEnum.PDF;
                                        break;

                                    case "image/png":
                                    case "image/jpg":
                                    case "image/jpeg":
                                        await formFile.CopyToAsync(inputStream);
                                        curriculo = new byte[inputStream.Length];
                                        inputStream.Seek(0, SeekOrigin.Begin);
                                        inputStream.Read(curriculo, 0, curriculo.Length);
                                        break;
                                }
                            }
                        }
                    }
                }
                var idIndicacaoUsuario = new IdIndicacaoUsuario();
                idIndicacaoUsuario = _sRSService.IndicarVaga(vagaInfo.idVaga, vagaInfo.nome, vagaInfo.email, vagaInfo.telefone, vagaInfo.deOndeConhece, vagaInfo.autorizou, vagaInfo.estaDisponivel, vagaInfo.linkedin).Result;
                ret.Convite = _sRSService.GerarConvite(vagaInfo.idVaga, idIndicacaoUsuario.IdIndicacao, vagaInfo.telefone, vagaInfo.nome, vagaInfo.linkedin, idIndicacaoUsuario.Usuario).Result;
                await _sRSService.AddConviteIndicacaoParcial(idIndicacaoUsuario.IdIndicacao, ret.Convite, curriculo, tipo);

                ret.Sucesso = true;

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.InnerException?.Message ?? e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("buscarCandidatosSRS")]
        public async Task<IActionResult> BuscasCandidatoSRS(string cpf)
        {
            var ret = _candidateService.GetCandidate(cpf);
            return Ok(ret);
        }

        [HttpPost("inserirCandidatoSRS")]
        public IActionResult InserirCandidatoSRS(SRSInsertCandidateParam param)
        {
            _candidateService.InsertCandidate(param);
            return Ok("Candidato registrado com sucesso");
        }

        [HttpGet("validaSeJaExisteEmail")]
        public IActionResult ValidaSeJaExisteEmail(string email1, string email2, string emailFoursys)
        {
            List<string> emails = new List<string>();

            if (email1 != null)
                emails.Add(email1);
            if (email2 != null)
                emails.Add(email2);
            if (emailFoursys != null)
                emails.Add(emailFoursys);

            var ret = _candidateService.ValidaSeJaExisteEmail(emails);
            return Ok(ret);
        }

        [HttpGet("GetEntrevista")]
        public IActionResult GetEntrevista(int candidateId)
        {
            var ret = _candidateService.GetEntrevista(candidateId);
            return Ok(ret);
        }

        [HttpGet("GetSoEntrevista")]
        public IActionResult GetSoEntrevista(int candidateId)
        {
            var ret = _candidateService.GetSoEntrevista(candidateId);
            return Ok(ret);
        }

        [HttpGet("GetSoEntrevistaId")]
        public IActionResult GetSoEntrevistaId(int entrevistaId, int candidateId)
        {
            var ret = _candidateService.GetSoEntrevistaId(entrevistaId, candidateId);
            return Ok(ret);
        }

        [HttpPost("InsertCandidateEntrevistaOld")]
        public IActionResult InsertCandidateEntrevistaOld(EntrevistaParam param)
        {
            switch (param.Area)
            {
                case 1:
                    _candidateService.EntrevistaRHInsert(param);
                    break;

                case 2:
                    _candidateService.InsertEntrevistaTecnica(param);
                    break;

                case 3:
                    _candidateService.InsertEntrevistaClienteGestor(param);
                    break;
            }
            return Ok("Inserido com sucesso");
        }

        [HttpPost("InsertCandidateEntrevista")]
        public async Task<IActionResult> InsertCandidateEntrevista(EntrevistaParam param)
        {
            var entrevistaId = await _candidateService.InsertCandidateEntrevista(param);

            return Ok(entrevistaId);
        }

        [HttpPost("EditarEntrevista")]
        public IActionResult EditarEntrevista(EntrevistaDTO param)
        {
            try
            {
                _candidateService.EditarEntrevista(param);
                return Ok("Editado com sucesso");
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("InsertOffboarding")]
        public IActionResult insertOffboarding(OffboardingDTO param)
        {
            try
            {
                var offBoardingId = _candidateService.insertOffboarding(param);
                return Ok(new { offBoardingId });
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("EditarOffboarding")]
        public IActionResult EditarOffboarding(OffboardingDTO param)
        {
            try
            {
                _candidateService.EditarOffboarding(param);
                return Ok("Editado com sucesso!");
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("GetOffboarding")]
        public IActionResult GetOffboarding(int candidateId)
        {
            try
            {
                var ret = _candidateService.GetOffboarding(candidateId);
                return Ok(ret);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("InsertSkillCandidate")]
        public IActionResult InsertSkillCandidate(CandidateSkillParam param)
        {
            try
            {
                _candidateService.InsertSkillCandidate(param);
                return Ok("Inserido com sucesso!");
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("InsertCandidateSkillEntrevista")]
        public IActionResult InsertCandidateSkillEntrevista(EntrevistaSkillParam param)
        {
            try
            {
                _candidateService.InsertCandidateSkillEntrevista(param);
                return Ok("Inserido com sucesso");
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("EditarSkillCandidate")]
        public IActionResult EditarSkillCandidate(CandidateSkillDTO param)
        {
            try
            {
                _candidateService.EditarSkillCandidate(param);
                return Ok("Skill editada com sucesso!");
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("RemoveSkillCandidate")]
        public IActionResult RemoveSkillCandidate(int skillId)
        {
            try
            {
                _candidateService.RemoveSkillCandidate(skillId);
                return Ok("Skill removida com sucesso!");
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("GetCandidateSkill")]
        public IActionResult GetCandidateSkills(int candidateId)
        {
            try
            {
                var ret = _candidateService.GetCandidateSkill(candidateId);
                return Ok(ret);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("GetCandidateSkillEntrevista")]
        public IActionResult GetCandidateSkillEntrevista(int candidateId, int entrevistaId)
        {
            try
            {
                var ret = _candidateService.GetCandidateSkillEntrevista(candidateId, entrevistaId);
                return Ok(ret);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("InsertOnboarding")]
        public IActionResult InsertOnboarding(OnboardingParam param)
        {
            try
            {
                _candidateService.InsertOnboarding(param);
                return Ok("Inserido com Sucesso");
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("GetOnboarding")]
        public IActionResult GetOnboarding(int candidateId)
        {
            try
            {
                var ret = _candidateService.GetOnboarding(candidateId);
                return Ok(ret);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("GetHistoricoCandidatoEntrevistas")]
        public IActionResult GetHistoricoCandidatoEntrevistas(int candidateId)
        {
            try
            {
                var ret = _candidateService.GetHistoricoCandidatoEntrevistas(candidateId);
                return Ok(ret);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("GetHistoricoCandidatoEntrevistaEspecifica")]
        public IActionResult GetHistoricoCandidatoEntrevistaEspecifica(int id)
        {
            try
            {
                var ret = _candidateService.GetHistoricoCandidatoEntrevistaEspecifica(id);
                return Ok(ret);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("HistoricoCandidatoEntrevista")]
        public IActionResult HistoricoCandidatoEntrevista(HistoricoCandidateEntrevistaDTO historico)
        {
            try
            {
                _candidateService.HistoricoCandidatoEntrevista(historico);
                return Ok("Inserido com Sucesso");
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("EditarOnboarding")]
        public IActionResult EditarOnboarding(OnboardingDTO param)
        {
            try
            {
                _candidateService.EditarOnboarding(param);
                return Ok("Editado com Sucesso!");
            }
            catch
            {
                throw;
            }
        }

        [HttpPost("RemoveOnboarding")]
        public IActionResult RemovoOnboarding(int id)
        {
            try
            {
                _candidateService.RemoveOnboarding(id);
                return Ok("Removido com sucesso!");
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("BuscarCargoSimples")]
        public IActionResult BuscarCargoSimples()
        {
            var ret = _candidateService.GetCargoSimples();
            return Ok(ret);
        }

        [HttpGet("BuscarCargo")]
        public IActionResult BuscarCargo(int cargoId)
        {
            var ret = _candidateService.GetCargo(cargoId);
            return Ok(ret);
        }

        [HttpGet("BuscarLocalDeTrabalho")]
        public IActionResult BuscarLocalDeTrabalho()
        {
            var ret = _candidateService.GetLocalDeTrabalho();
            return Ok(ret);
        }

        [HttpGet("UnidadeFourmakersToSRS")]
        public IActionResult UnidadeFourmakersToSRS(int idUnidade)
        {
            if (idUnidade <= 0 || idUnidade > 8)
            {
                return BadRequest("Não existe uma unidade com esse id");
            }
            ;
            if (idUnidade == 7)
            {
                idUnidade = 8;
            }
            if (idUnidade == 6)
            {
                return BadRequest("Unidade 6 candidato não é uma unidade valida");
            }
            var ret = _candidateService.UnidadeFourmakersToSRS(idUnidade);
            return Ok(ret);
        }

        [HttpGet("BuscarVagaSrs")]
        public async Task<IActionResult> BuscarVagaSRS(string token)
        {
            try
            {
                var vagas = await _sRSService.BuscarVagaSRS(token);
                return Ok(vagas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ocorreu um erro: {ex.Message}");
            }
        }

        [HttpPost("EditarCriarVagaFourmakersSRS")]
        public async Task<IActionResult> EditarCriarVagaFourmakersSRS(CriarVagasSRSParam param)
        {
            try
            {
                var retorno = new ApiGenericResult<CriarVagasSRSParam>() { Mensagem = $"Vaga {(param.IdVaga is null ? "cadastrada" : "alterada")} no SRS com sucesso." };
                string token = _aspNetUser.GetUsuarioLogado().Token;
                var criarVagasSRSResult = await _sRSService.EditarCriarVagasFourmakersSRS(param, token);
                retorno.Retorno = criarVagasSRSResult;

                return Ok(retorno);
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<string> { Sucesso = false, Mensagem = "Acesso não autorizado!" });
            }
            catch (ArgumentException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpGet("ObterVagaPorId")]
        public async Task<IActionResult> ObterVagaPorId(int vagaId)
        {
            try
            {
                var retorno = new ApiGenericResult<CriarVagasSRSParam>();
                string token = _aspNetUser.GetUsuarioLogado().Token;
                var criarVagasSRSResult = await _sRSService.ObterVagaPorId(token, vagaId);
                retorno.Retorno = criarVagasSRSResult;

                return Ok(retorno);
            }
            catch (UnauthorizedAccessException e)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message});
            }
            catch (ArgumentException e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiGenericResult<string> { Sucesso = false, Mensagem = e.Message });
            }
        }

        [HttpPost("InserirSkillVagaFourmakersSRS")]
        public async Task<IActionResult> InserirSkillVagaFourmakersSRS(SkillVagaParam param)
        {
            var ret = await _sRSService.InserirSkillVagaFourmakersSRS(param);
            return Ok(ret);
        }

        [HttpPost("EditarSkillVagaFourmakersSRS")]
        public async Task<IActionResult> EditarSkillVagaFourmakersSRS(SkillVagaParam param)
        {
            var ret = await _sRSService.EditarSkillVagaFourmakersSRS(param);
            return Ok(ret);
        }

        [HttpPost("RemoverSkillVagaFourmakersSRS")]
        public async Task<IActionResult> RemoverSkillVagaFourmakersSRS(int skillId)
        {
            var ret = await _sRSService.RemoverSkillVagaFourmakersSRS(skillId);
            return Ok(ret);
        }

        [HttpGet("BuscaSkillsVagaFourmakersSRS")]
        public async Task<IActionResult> BuscaSkillsVagaFourmakersSRS(int vagaId)
        {
            var ret = await _sRSService.BuscaSkillsVagaFourmakersSRS(vagaId);
            return Ok(ret);
        }

        [HttpPost("AlterarStatusCandidaturaFourmakersSRS")]
        public async Task<IActionResult> AlterarStatusCandidaturaFourmakersSRS(AlterarStatusCandidaturaParam status)
        {
            string email = _aspNetUser.GetUsuarioLogado().Email;
            var ret = await _candidateService.AlterarStatusCandidatura(status, email);
            return Ok(ret);
        }

        [HttpGet("GetColaboradorAtivoOuInativoPorCPF")]
        public async Task<ActionResult> GetColaboradorAtivoOuInativoPorCPF(string colaboradorCpf)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                result.Retorno = await _sRSService.GetColaboradorAtivoOuInativoPorCPF(colaboradorCpf, _aspNetUser.GetUsuarioLogado().Token);
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message.ToString();
                return StatusCode(500, result);
            }
        }

        [HttpPost("CadastroCandidatoFourmakersLinkedin")]
        public async Task<ActionResult> CadastroCandidatoFourmakersLinkedin(CadastroCandidatoLinkedinInput request)
        {
            var result = new ApiGenericResult<int>();
            try
            {
                result.Retorno = await _candidateService.CadastroCandidatoFourmakersLinkedin(request);
                return Ok(result);
            }
            catch (ValidationException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message.ToString();
                return StatusCode(401, result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno no servidor";
                return StatusCode(500, result);
            }
        }

        [HttpGet("ValidarUsuarioLinkedin")]
        public async Task<ActionResult> ValidarUsuarioLinkedin(string username, int? userId = null)
        {
            var result = new ApiGenericResult<ValidarUsuarioLinkedinResponse>();
            try
            {
                result.Retorno = await _sRSService.ValidarUsuarioLinkedinService(userId, username);
                return Ok(result);
            }
            catch (ValidationException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message.ToString();
                return StatusCode(401, result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno no servidor";
                return StatusCode(500, result);
            }
        }

        [HttpPost("CadastrarAtualizarPreAprovarCandidadoLinkedin")]
        public async Task<ActionResult> CadastrarAtualizarPreAprovarCandidadoLinkedin(CadastrarAtualizarPreAprovarCandidadoLinkedinInput model)
        {
            var result = new ApiGenericResult<CadastroCandidatoInputResponse>();
            try
            {
                result.Retorno = await _candidateService.CadastrarAtualizarPreAprovarCandidadoService(model);
                return Ok(result);
            }
            catch (ValidationException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message.ToString();
                return StatusCode(401, result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno no servidor";
                return StatusCode(500, result);
            }
        }

        [HttpPost("EnviarBancoTalentoFourmakers")]
        [AllowAnonymous]
        public async Task<ActionResult> EnviarBancoTalentoFourmakers(EnviarBancoTalentoFourmakersInput model)
        {
            var result = new ApiGenericResult<StatusResult>();
            try
            {
                // Verifica se existe o header de autorização Bearer com o valor "j95TOgg85FSsJ8EW3IIw4KvJ"
                var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ") || authorizationHeader != "Bearer j95TOgg85FSsJ8EW3IIw4KvJ")
                {
                    result.Sucesso = false;
                    result.Mensagem = "Não autorizado";
                    return StatusCode(401, result);
                }

                _candidateService.EnviarBancoTalentoFourmakers(model.UrlLinkedin, model.CodVaga, model.Email, model.Telefone);
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno no servidor";
                return StatusCode(500, result);
            }
        }

        [HttpPost("CadastroCandidatoLinkedin")]
        public async Task<ActionResult> CadastroCandidatoLinkedln(CadastroCandidatoInput request)
        {
            var result = new ApiGenericResult<CadastroCandidatoInputResponse>();
            try
            {
                result.Retorno = await _candidateService.CadastroCandidatoLinkedin(request);
                return Ok(result);
            }
            catch (ValidationException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message.ToString();
                return StatusCode(401, result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno no servidor";
                return StatusCode(500, result);
            }
        }

        [HttpPost("AtualizarCandidatoLinkedin")]
        public async Task<ActionResult> AtualizarCandidatoLinkedln(AtualizaCadastroCandidatoInput request)
        {
            var result = new ApiGenericResult<AtualizaCadastroCandidatoInputResponse>();
            try
            {
                result.Retorno = await _candidateService.AtualizaCurriculoCandidate(request);
                return Ok(result);
            }
            catch (ValidationException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message.ToString();
                return StatusCode(401, result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno no servidor";
                Console.WriteLine("Falha no AtualizarCandidatoLinkedln: " + e.Message + "\nStackTrace: " + e.StackTrace);
                return StatusCode(500, result);
            }
        }

        [HttpGet("GetRelatorioCandidatosRaw")]
        public ActionResult GetRelatorioCandidatosRaw()
        {
            try
            {
                return Ok(_candidateService.GetRelatorioCandidate(""));
            }
            catch (Exception e)
            {
                return StatusCode(500, "Erro interno no servidor");
            }
        }

        [HttpPost("AlterarCategoriaHabilidade")]
        public async Task<ActionResult> AlterarCategoriaHabilidade(SRSAlterarCategoriaHabilidadeParam param)
        {
            var result = new ApiGenericResult<bool>();
            try
            {
                result.Retorno = await _sRSService.AlterarCategoriaHabilidade(param);
                return Ok(result);
            }
            catch (ValidationException e)
            {
                result.Sucesso = false;
                result.Mensagem = e.Message.ToString();
                return StatusCode(500, result);
            }
            catch (Exception e)
            {
                result.Sucesso = false;
                result.Mensagem = "Erro interno no servidor";
                Console.WriteLine("Falha no AlterarCategoriaHabilidade: " + e.Message + "\nStackTrace: " + e.StackTrace);
                return StatusCode(500, result);
            }
        }

        [HttpPost("ListarIndicacoesPremiadas")]
        public async Task<ActionResult<List<IndicacaoPremiadaParcialDTO>>> ListarIndicacoesPremiadas(ListarIndicacaoPremiadaParcialParam param)
        {
            try
            {
                var ret = await _sRSService.ListarIndicacoesPremiadas(param);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("EditarIndicacoesPremiadas")]
        public async Task<ActionResult<IndicacaoPremiadaParcialDTO>> EditarIndicacoesPremiadas(EditarIndicacaoPremiadaParcialParam param)
        {
            try
            {
                var ret = await _sRSService.EditarIndicacaoPremiadaParcial(param);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("BuscarIndicacaoPremiadaPorId")]
        public async Task<ActionResult<IndicacaoPremiadaParcialDTO>> BuscarIndicacaoPremiadaPorId(int id)
        {
            try
            {
                var ret = await _sRSService.BuscarIndicacaoPremiadaPorId(id);
                return Ok(ret);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("BuscarVagasDoUltimoAno")]
        public async Task<ActionResult<List<JobOrderDTO>>> BuscarVagasDoUltimoAno()
        {
            try
            {
                return Ok(await _sRSService.BuscarVagasDoUltimoAno(_aspNetUser.GetUsuarioLogado().Token));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Mapeia o parâmetro 'org' (string) para o orgId (int) correspondente.
        /// </summary>
        private int? MapOrgToOrgId(string org)
        {
            if (string.IsNullOrWhiteSpace(org))
                return null;

            var orgLower = org.Trim().ToLowerInvariant();
            
            return orgLower switch
            {
                "fourmakers" => (int)EnumORG.FOURMAKERS_1,
                "foursys" => (int)EnumORG.FOURSYS_2,
                "bwg" => (int)EnumORG.BWG_3,
                "numen" => (int)EnumORG.NUMEN_4,
                "showcase" => (int)EnumORG.SHOWCASE_5,
                "novacoop" => (int)EnumORG.NOVACOOP_2882_6,
                "fmu" => (int)EnumORG.FMU_7,
                "trial" => (int)EnumORG.TRIAL_8,
                "royal" => (int)EnumORG.ROYAL_9,
                _ => null
            };
        }

    }
}