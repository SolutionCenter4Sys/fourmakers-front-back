using Colaboracao.Core.Interfaces;
using Competencia.API.DTOs;
using Competencia.Domain.Interfaces.Services;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Hobby;
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
    public class HobbieController : ControllerBase
    {
        private IHobbyService _hobbyService;
        private readonly IVerificaSeCpfESistemico _verificaCpfSistemico;

        public HobbieController(IHobbyService hobbyService, IVerificaSeCpfESistemico verificaCpfSistemico)
        {
            _hobbyService = hobbyService;
            _verificaCpfSistemico = verificaCpfSistemico;
        }

        [AllowAnonymous]
        [HttpGet("ListarHobbie")]
        public ActionResult<ListaHobbyResult> ListarHobbie(string busca, int cursor, int limite)
        {
            var ret = new ListaHobbyResult();
            try
            {
                var retService = _hobbyService.ListarHobbies(busca, cursor, limite);
                ret.Hobbie = retService.Select(x => new ItemPerfilDTO
                {
                    Descricao = x.HobbyDTO.Descricao,
                    Id = x.HobbyDTO.IdHobby
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

        [HttpPost("AdicionarHobbie")]
        public ActionResult<HobbyResult> AdicionarHobbie(ParamCriaHobby param)
        {
            var ret = new HobbyResult();
            try
            {
                var retHobby = _hobbyService.InserirHobby(param.Descricao);
                ret.Hobbie = new ItemPerfilDTO
                {
                    Descricao = retHobby.HobbyDTO.Descricao,
                    Id = retHobby.HobbyDTO.IdHobby
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

        [HttpPost("AdicionarHobbieColaborador")]
        public ActionResult<HobbyColaboradorResult> AdicionarHobbieColaborador(List<ParamHobbyColaborador> param)
        {
            var ret = new HobbyColaboradorResult();
            ret.Respostas = new List<ItemPerfilResult>();
            try
            {
                foreach (var item in param)
                {
                    try
                    {
                        var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(item.Cpf);

                        var retHobbyColaborador = _hobbyService.InserirHobbieColaborador(item.Id, cpfRequest);
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

        [HttpPost("RemoverHobbieColaborador")]
        public ActionResult<StatusResult> RemoverHobbieColaborador(ParamHobbyColaborador param)
        {
            var ret = new StatusResult();
            try
            {
                var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);

                _hobbyService.RemoverHobbieColaborador(param.Id, cpfRequest);
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

        [HttpGet("ListarHobbiesColaborador")]
        public ActionResult<ListaHobbyColaboradorResult> ListarHobbiesColaborador(string CpfColaborador)
        {
            var ret = new ListaHobbyColaboradorResult();
            try
            {
                var retService = _hobbyService.ListarHobbiesColaborador(CpfColaborador);
                ret.Hobbies = retService.Select(x => new HobbyColaboradorDTO
                {
                    Id = x.HobbyColaboradorDTO.Id,
                    Hobbie = new ItemPerfilDTO
                    {
                        Id = x.HobbyColaboradorDTO.Hobbie.Id,
                        Descricao = x.HobbyColaboradorDTO.Hobbie.Descricao
                    },
                    Data = x.HobbyColaboradorDTO.Data
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

        [HttpGet("GetHobbyById")]
        public ActionResult<GetItemPerfilResult> GetHobbyById(long id)
        {
            var ret = new GetItemPerfilResult();
            try
            {
                var retService = _hobbyService.GetHobbiesById(id);
                if (retService != null)
                {
                    ret.ItemPerfil = new ItemPerfilDTO
                    {
                        Descricao = retService.HobbyDTO.Descricao,
                        Id = retService.HobbyDTO.IdHobby
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

        [HttpPost("MergeHobbieColaborador")]
        public async Task<StatusResult> MergeHobbieColaborador(MergeItemPerfilParam param)
        {
            var ret = new StatusResult();
            try
            {
                var cpfRequest = _verificaCpfSistemico.VerificaCpfSistemico(param.Cpf);

                var lstItensColab = _hobbyService.ListarHobbiesColaborador(cpfRequest);
                var lstInsert = new List<AdicionarRemoverItemParam>();
                var lstDelete = new List<AdicionarRemoverItemParam>();

                var listItensColabDTO = new List<HobbyColaboradorDTO>();

                foreach (var item in lstItensColab)
                {
                    listItensColabDTO.Add(item.HobbyColaboradorDTO);
                }

                lstDelete = listItensColabDTO.Where(x => param.Itens.Where(y => y.Id == x.Hobbie.Id).Count() == 0).Select(x =>
                {
                    return new AdicionarRemoverItemParam
                    {
                        Id = x.Hobbie.Id,
                        NivelId = null,
                        Cpf = cpfRequest
                    };
                }).ToList();

                lstInsert = param.Itens.Where(x => listItensColabDTO.Where(y => y.Hobbie.Id == x.Id).Count() == 0).Select(x =>
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
                        _hobbyService.InserirHobbieColaborador(item.Id, cpfRequest);
                    }
                    catch { /*Ignora*/ }
                }

                foreach (var item in lstDelete)
                {
                    try
                    {
                        _hobbyService.RemoverHobbieColaborador(item.Id, cpfRequest);
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