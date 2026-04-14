using Colaboracao.Core.Interfaces;
using Colaboracao.Helper;
using Competencia.API.DTOs;
using Competencia.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Interesse;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Competencia.API.Controllers
{
    [Authorize]
    [Route("api/Competencia/[controller]")]
    [ApiController]
    [LogAction]
    public class InteresseController : ControllerBase
    {
        private IInteresseService _interesseService;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public InteresseController(IInteresseService interesseService, IVerificaSeCpfESistemico verificaCpfSistemico, IAspNetUser aspNetUser)
        {
            _interesseService = interesseService;
            _verificaCpfSistemico = verificaCpfSistemico;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [AllowAnonymous]
        [HttpGet("ListarInteresse")]
        public ActionResult<ListaInteresseResult> ListarInteresse(string busca, int cursor, int limite)
        {
            var ret = new ListaInteresseResult();
            try
            {
                var retService = _interesseService.ListarInteresses(busca, cursor, limite);
                ret.Interesse = retService.Select(x => new ItemPerfilDTO
                {
                    Descricao = x.Descricao,
                    Id = x.IdInteresse
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

        [HttpPost("AdicionarInteresse")]
        public ActionResult<InteresseResult> AdicionarInteresse(ParamCriaInteresse param)
        {
            var ret = new InteresseResult();
            try
            {
                var retInteresse = _interesseService.InserirInteresse(param.Descricao);
                ret.Interesse = new ItemPerfilDTO()
                {
                    Descricao = retInteresse.Descricao,
                    Id = retInteresse.IdInteresse
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

        [HttpPost("AdicionarInteresseColaborador")]
        public ActionResult<InteresseColaboradorResult> AdicionarInteresseColaborador(List<ParamInteresseColaborador> param, bool minhaJornada = false)
        {
            var ret = new InteresseColaboradorResult();
            ret.Respostas = new List<ItemPerfilResult>();

            try
            {
                foreach (var item in param)
                {
                    try
                    {
                        var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(item.Cpf);

                        var retInteresseColaborador = _interesseService.InserirInteresseColaborador(item.Id, cpfRequest, item.InteresseAtivo, item.TipoId.ToInt(), item.SkillId, minhaJornada, item.GestorExternoPerfil, item.NivelId, _usuarioLogado.Cpf);
                        ret.Respostas.Add(new ItemPerfilResult
                        {
                            Sucesso = true,
                            ItemId = item.Id
                        });
                    }
                    catch (Exception e)
                    {
                        ret.Respostas.Add(new ItemPerfilResult
                        {
                            Sucesso = false,
                            ItemId = item.Id,
                            Mensagem = e.Message
                        });
                    }
                }
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

        [HttpPost("RemoverInteresseColaborador")]
        public ActionResult<StatusResult> RemoverInteresseColaborador(ParamInteresseColaborador param)
        {
            var ret = new StatusResult();
            try
            {
                var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);

                _interesseService.RemoverInteresseColaborador(param.Id, cpfRequest);
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

        [HttpGet("ListarInteressesColaborador")]
        public ActionResult<ListaInteresseColaboradorResult> ListarInteressesColaborador(string CpfColaborador)
        {
            var ret = new ListaInteresseColaboradorResult();
            try
            {
                var retService = _interesseService.ListarInteressesColaborador(CpfColaborador);
                ret.Interesses = retService;
                return Ok(ret);
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return StatusCode(500, ret);
            }
        }

        [HttpGet("GetInteresseById")]
        public ActionResult<GetItemPerfilResult> GetInteresseById(long id)
        {
            var ret = new GetItemPerfilResult();
            try
            {
                var retService = _interesseService.GetInteressesById(id);
                if (retService != null)
                {
                    ret.ItemPerfil = new ItemPerfilDTO
                    {
                        Descricao = retService.Descricao,
                        Id = retService.IdInteresse
                    };
                }
                else
                {
                    ret.ItemPerfil = new ItemPerfilDTO
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

        [HttpPost("MergeInteresseColaborador")]
        public async Task<StatusResult> MergeInteresseColaborador(MergeItemPerfilParam param)
        {
            var ret = new StatusResult();
            try
            {
                var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);

                var lstItensColab = _interesseService.ListarInteressesColaborador(cpfRequest);
                var lstInsert = new List<AdicionarRemoverItemParam>();
                var lstDelete = new List<AdicionarRemoverItemParam>();

                lstDelete = lstItensColab.Where(x => param.Itens.Where(y => y.Id == x.Interesse.Id).Count() == 0).Select(x =>
                {
                    return new AdicionarRemoverItemParam
                    {
                        Id = x.Interesse.Id,
                        NivelId = null,
                        Cpf = cpfRequest
                    };
                }).ToList();

                lstInsert = param.Itens.Where(x => lstItensColab.Where(y => y.Interesse.Id == x.Id).Count() == 0).Select(x =>
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
                        _interesseService.InserirInteresseColaborador(item.Id, cpfRequest, item.InteresseAtivo, item.TipoId.ToInt(), item.SkillId, false, string.Empty, 0, _usuarioLogado.Cpf);
                    }
                    catch { /*Ignora*/ }
                }

                foreach (var item in lstDelete)
                {
                    try
                    {
                        _interesseService.RemoverInteresseColaborador(item.Id, cpfRequest);
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
    }
}