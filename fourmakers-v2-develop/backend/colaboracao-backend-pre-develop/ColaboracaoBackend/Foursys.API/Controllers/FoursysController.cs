using DataTransferObject.Domain.Cargo;
using DataTransferObject.Domain.Diretoria;
using DataTransferObject.Domain.Foursys;
using Foursys.API.DTOs;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;

namespace Foursys.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [LogAction]
    public class FoursysController : ControllerBase
    {
        private IFoursysService _foursysService;
        private UsuarioLogadoDTO  _usuarioLogado;

        public FoursysController(IFoursysService foursysService, IAspNetUser aspNetUser)
        {
            _foursysService = foursysService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("BuscarCargo")]
        public BuscaCargoResult BuscarCargo(string busca, int cursor, int limite)
        {
            try
            {
                return _foursysService.BuscarCargos(busca, cursor, limite);
            }
            catch (Exception e)
            {
                var ret = new BuscaCargoResult();
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

        [HttpGet("EncontrarCargo")]
        public CargoResult EncontrarCargo(int id)
        {
            var ret = new CargoResult();
            try
            {
                ret.Cargo = _foursysService.BuscarCargo(id);
                return ret;
            }
            catch (Exception e)
            {
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

        [HttpPost("IncluirCargo")]
        public CargoResult IncluirCargo(ParamIncluirCargo param)
        {
            var ret = new CargoResult();
            try
            {
                ret.Cargo = _foursysService.InsereCargo(param.Descricao);
                return ret;
            }
            catch (Exception e)
            {
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

        [HttpPost("AlteraCargo")]
        public CargoResult AlteraCargo(ParamAlteraCargo param)
        {
            var ret = new CargoResult();
            try
            {
                ret.Cargo = _foursysService.AlteraCargo(param.Id, param.Cargo);
                return ret;
            }
            catch (Exception e)
            {
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

        [HttpPost("DeletaCargo")]
        public CargoResult DeletaCargo(ParamDeletaCargo param)
        {
            var ret = new CargoResult();
            var cargo = new CargoDTO() { Cargo = param.Cargo, Id = param.Id };
            try
            {
                ret.Sucesso = _foursysService.DeletaCargo(param.Id, param.Cargo);
                ret.Cargo = cargo;
                return ret;
            }
            catch (Exception e)
            {
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

        [HttpGet("BuscarDiretoria")]
        public BuscaDiretoriaResult BuscarDiretoria(string busca, int cursor, int limite)
        {
            try
            {
                return _foursysService.BuscarDiretorias(busca, cursor, limite);
            }
            catch (Exception e)
            {
                var ret = new BuscaDiretoriaResult();
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

        [HttpGet("RelatorioAcessoFourmakers")]
        public RelatorioUsuarioAcessoResult RelatorioAcessoFourmakers()
        {
            var ret = new RelatorioUsuarioAcessoResult();
            try
            {
                ret.Usuarios = _foursysService.ListarAcessoUsuarios();
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("ListarUnidades")]
        public ListaUnidadesResult ListarUnidades()
        {
            var ret = new ListaUnidadesResult();
            try
            {
                ret.ListaUnidades = _foursysService.ListarUnidades();
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpGet("ListarUnidadesPorOrg")]
        public async Task<ListaUnidadesResult> ListarUnidadesPorOrg(int orgId)
        {
            var ret = new ListaUnidadesResult();
            try
            {
                var retService = await _foursysService.ListarUnidadesPorOrgRestricao(orgId, _usuarioLogado.Cpf);
                ret = retService;
                return ret;
            }
            catch (Exception e)
            {
                ret.Sucesso = false;
                ret.Mensagem = e.Message;
                return ret;
            }
        }

        [HttpPost("EnviarEmailCadastroIncompleto")]
        public IActionResult EnviarEmailCadastroIncompleto(IEnumerable<UsuarioAcessoDTO> usuarios)
        {
            try
            {
                EnviarEmailCadastroIncompletoResult ret = _foursysService.EnviarEmailCadastroIncompleto(usuarios.Where(x => x.Status == "Incompleto"));
                if (ret.Sucesso) return Ok(ret);
                return BadRequest(ret);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("ListaColaboradoresOrgId")]
        public List<ColaboradoresTotaisOrgIdResult> ListaColaboradoresOrgId(int? org, int page = 1, int pageSize = 10)
        {
            try
            {
                var retListaColaboradoresOrgId = _foursysService.ListaColaboradoresOrgId(org ?? 0, page, pageSize);
                return retListaColaboradoresOrgId;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
