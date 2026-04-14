using Colaboracao.Core.Interfaces;
using Competencia.API.DTOs;
using Competencia.Domain.Impl.Services;
using Competencia.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Competencia;
using DataTransferObject.Domain.Nivel;
using DataTransferObject.Domain.Softskill;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Logs.Infra.Attributes;

namespace Competencia.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Competencia/[controller]")]
    [LogAction]
    public class SoftSkillController : ControllerBase
    {
        private ISoftskillService _softSkillService;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;
        private readonly IAspNetUser _aspNetUser;

        public SoftSkillController(ISoftskillService softSkillService, IVerificaSeCpfESistemico verificaCpfSistemico, IAspNetUser aspNetUser)
        {
            _softSkillService = softSkillService;
            _verificaCpfSistemico = verificaCpfSistemico;
            _aspNetUser = aspNetUser;
        }

        [AllowAnonymous]
        [HttpGet("ListarSoftSkill")]
        public ActionResult<ListaSoftskillComNivelResult> ListarSoftSkill(string busca, int cursor, int limite)
        {
            var ret = new ListaSoftskillComNivelResult();

            try
            {
                ret.Retorno = _softSkillService.ListarSoftSkill(busca, cursor, limite);
                ret.Nivel = _softSkillService.ListarNivelSoftskill();
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("GetSoftskillById")]
        public ActionResult<ApiGenericResult<ItemPerfilDTO>> GetSoftskillById(long id)
        {
            var ret = new ApiGenericResult<ItemPerfilDTO>();
            try
            {
                var retService = _softSkillService.GetSoftskillById(id);
                if (retService.Descricao != null)
                {
                    ret.Retorno = new ItemPerfilDTO
                    {
                        Id = id,
                        Descricao = retService.Descricao,
                        Pendente = retService.Pendente
                    };
                }
                else
                {
                    ret.Retorno = new ItemPerfilDTO
                    {
                        Descricao = null,
                        Id = id
                    };
                    ret.Sucesso = false;
                    ret.Mensagem = "Item de perfil não encontrado.";
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

        [HttpPost("AdicionarSoftskill")]
        public ActionResult<ApiGenericResult<ItemPerfilDTO>> AdicionarSoftSkill(AddSoftskillParam param)
        {
            var ret = new ApiGenericResult<ItemPerfilDTO>();
            try
            {
                var retSoftskill = _softSkillService.AdicionarSoftSkill(param.Descricao.ToUpper(), _aspNetUser.GetUsuarioLogado().Cpf);
                ret.Retorno = new ItemPerfilDTO()
                {
                    Descricao = retSoftskill.Descricao,
                    Id = retSoftskill.Id
                };
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("ListarSoftskillColaborador")]
        public ActionResult<ApiGenericResult<List<SoftskillColaboradorDTO>>> ListarSoftskillColaborador(string cpfColaborador)
        {
            var ret = new ApiGenericResult<List<SoftskillColaboradorDTO>>();
            try
            {
                ret.Retorno = _softSkillService.ListarSoftskillColaborador(cpfColaborador);

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AdicionarSoftskillColaborador")]
        public ActionResult<ApiGenericResult<List<ItemPerfilResult>>> AdicionarSoftskillColaborador([FromBody] List<AddSoftskillColabParam> param, [FromQuery] bool minhaJornada = false)
        {
            var ret = new ApiGenericResult<List<ItemPerfilResult>>();
            try
            {
                var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                ret.Retorno = _softSkillService.AdicionarSoftskillColaboradorEmLote(param, usuarioLogado.Token, minhaJornada, usuarioLogado.Cpf);

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

        [HttpPost("RemoverSoftskillColaborador")]
        public ActionResult<StatusResult> RemoverSoftskillColaborador(RemoveSoftskillColabParam param)
        {
            var ret = new StatusResult();
            try
            {
                var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);

                _softSkillService.RemoveSoftskillColaborador(param.SoftSkillId, cpfRequest);
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

        [HttpGet("GetGroupSoftSkillById")]
        public async Task<ActionResult<List<CompetenciaGroupDTO>>> GetGroupSoftSkillById(string ids)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return BadRequest("Informe ao menos um id.");

            // separa por vírgula → converte para long
            var listaIds = ids
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => long.Parse(x.Trim()))
                .ToList();

            var competencias = await _softSkillService.GetGroupSoftSkillByIds(listaIds);

            return Ok(competencias);
        }

        [HttpGet("ListarNivelSoftskill")]
        public ActionResult<ApiGenericResult<List<NivelDTO>>> ListarNivelSoftSkill()
        {
            var ret = new ApiGenericResult<List<NivelDTO>>();
            try
            {
                var retService = _softSkillService.ListarNivelSoftskill();

                ret.Retorno = (retService);

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("AtualizaSoftSkillColaborador")]
        public ApiGenericResult<long> AtualizaSoftSkillColaborador([FromBody] AdicionarRemoverItemParam param, [FromQuery] bool minhaJornada)
        {
            var ret = new ApiGenericResult<long>();
            try
            {
                var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);

                try
                {
                    var usuarioLogado = _aspNetUser.GetUsuarioLogado();

                    var retService = _softSkillService.AlterarSoftskillColaborador(cpfRequest, param.Id, param.NivelId, param.GestorExternoPerfil, minhaJornada, usuarioLogado.Cpf);

                    ret.Retorno = retService.Id;
                }
                catch (Exception e)
                {
                    ret = new ApiGenericResult<long>()
                    {
                        Sucesso = false,
                        Retorno = param.Id,
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

        [HttpPost("MergeSoftSkillColaborador")]
        public async Task<StatusResult> MergeSoftSkillColaborador(MergeItemPerfilParam param)
        {
            var ret = new StatusResult();

            var usuarioLogado = _aspNetUser.GetUsuarioLogado();

            try
            {
                var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);

                var niveis = _softSkillService.ListarNivelSoftskill();
                var lstItensColab = _softSkillService.ListarSoftskillColaborador(cpfRequest);
                var lstInsert = new List<AdicionarRemoverItemParam>();
                var lstDelete = new List<AdicionarRemoverItemParam>();

                var listItensColabDTO = new List<SoftskillColaboradorDTO>();

                foreach (var item in lstItensColab)
                {
                    listItensColabDTO.Add(item);
                }

                lstDelete = listItensColabDTO.Where(x => param.Itens.Where(y => y.Id == x.SoftSkill.Id).Count() == 0).Select(x =>
                {
                    return new AdicionarRemoverItemParam
                    {
                        Id = x.SoftSkill.Id,
                        NivelId = x.Nivel?.Id,
                        Cpf = cpfRequest
                    };
                }).ToList();

                lstInsert = param.Itens.Where(x => listItensColabDTO.Where(y => y.SoftSkill.Id == x.Id).Count() == 0).Select(x =>
                {
                    return new AdicionarRemoverItemParam
                    {
                        Id = x.Id,
                        NivelId = x.NivelId,
                        Cpf = cpfRequest
                    };
                }).ToList();

                foreach (var item in lstInsert)
                {
                    try
                    {
                        if (item.NivelId != null && !niveis.Any(x => x.Id == item.NivelId))
                        {
                            item.NivelId = null;
                        }
                        _softSkillService.AddSoftskillColaborador(item.Id, item.NivelId, cpfRequest, usuarioLogado.Token, false, string.Empty, usuarioLogado.Cpf);
                    }
                    catch { /*Ignora*/ }
                }

                foreach (var item in lstDelete)
                {
                    try
                    {
                        _softSkillService.RemoveSoftskillColaborador(item.Id, cpfRequest);
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

        [AllowAnonymous]
        [HttpGet("ListarSoftSkillsNaoAtribuidas")]
        public ActionResult<ListaSoftskillResult> ListarSoftSkillsNaoAtribuidas(string busca, int cursor, int limite)
        {
            var cpfLogado = _aspNetUser.GetUsuarioLogado().Cpf;

            var ret = new ListaSoftskillResult();

            try
            {
                ret = _softSkillService.ListarSoftSkillsNaoAtribuidas(busca, cursor, limite, cpfLogado);

                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpPost("GetSoftSkillInfoByDescricao")]
        public async Task<ActionResult> GetSoftSkillInfoByDescricao(GetSoftSkillInfoByDescricao param)
        {
            try
            {
                return Ok(await _softSkillService.GetSoftSkillInfoByDescricao(param.SoftSkills));
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message + "\nStackTrace: " + err.StackTrace);
                return StatusCode(500, "Erro interno servidor");
            }
        }
    }
}