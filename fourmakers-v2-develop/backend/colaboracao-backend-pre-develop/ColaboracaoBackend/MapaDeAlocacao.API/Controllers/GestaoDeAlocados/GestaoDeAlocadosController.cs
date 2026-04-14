using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.GestaoDeAlocados;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MapaDeAlocacao.API.Controllers.GestaoDeAlocados
{
    [Authorize]
    [Route("api/[controller]")]
    [HandleException]
    [ApiController]
    [LogAction]
    public class GestaoDeAlocadosController : ControllerBase
    {
        private readonly IGestaoAlocadosService _gestaoAlocadosService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public GestaoDeAlocadosController(IGestaoAlocadosService gestaoAlocadosService, IAspNetUser aspNetUser)
        {
            _gestaoAlocadosService = gestaoAlocadosService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpGet("ListarClienteOrgDaGestaoDeAlocados")]
        public async Task<ActionResult> ListarClienteOrgDaGestaoDeAlocados(int limite, int cursor, bool semPerfil, string busca)
        {
            var clientes = await _gestaoAlocadosService.ListarClienteOrgDaGestaoDeAlocados(_usuarioLogado.Cpf, limite, cursor, semPerfil, busca, _usuarioLogado.OrgId);
            return Ok(clientes);
        }

        [HttpGet("ListarPermanencias")]
        public async Task<ActionResult> ListarPermanencias()
        {
            var permanencias = await _gestaoAlocadosService.ListarPermanencias(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(permanencias);
        }

        [HttpGet("ListarProfissionaisLocalidades")]
        public async Task<ActionResult> ListarProfissionaisLocalidades()
        {
            var permanencias = await _gestaoAlocadosService.ListarProfissionaisLocalidades(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(permanencias);
        }

        [HttpGet("ListarModelosTrabalho")]
        public async Task<ActionResult> ListarModelosTrabalho()
        {
            var permanencias = await _gestaoAlocadosService.ListarModelosTrabalho(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(permanencias);
        }

        [AllowAnonymous]
        [HttpGet("ListarModelosTrabalhoPublico")]
        public async Task<ActionResult> ListarModelosTrabalhoPublico()
        {
            var modelos = await _gestaoAlocadosService.ListarModelosTrabalhoPublico();
            return Ok(modelos);
        }

        [HttpGet("ListarGestoresEPerfisDaGestaoDeAlocados")]
        public async Task<ActionResult> ListarGestoresEPerfisDaGestaoDeAlocados(int limite, int cursor, string busca, string codigoCliente, bool semPerfil, bool semAreaAtuacao)
        {
            var perfis = await _gestaoAlocadosService.ListarGestoresEPerfisDaGestaoDeAlocados(_usuarioLogado.Cpf, limite, cursor, busca, codigoCliente, semPerfil, semAreaAtuacao, _usuarioLogado.OrgId);
            return Ok(perfis);
        }

        [HttpGet("ListarRateCardsDosPerfisPorCliente")]
        public async Task<ActionResult> ListarRateCardsDosPerfisPorCliente(int limite, int cursor, string busca, string codigoCliente)
        {
            var perfis = await _gestaoAlocadosService.ListarRateCardsDosPerfisPorCliente(_usuarioLogado.Cpf, limite, cursor, busca, codigoCliente, _usuarioLogado.OrgId);
            return Ok(perfis);
        }

        [HttpGet("ListarRateCardsDosPerfisHistoricoPorPerfilId")]
        public async Task<ActionResult> ListarRateCardsDosPerfisHistoricoPorPerfilId(int limite, int cursor, Guid gestorExternoPerfilId)
        {
            var perfis = await _gestaoAlocadosService.ListarRateCardsDosPerfisHistoricoPorPerfilId(_usuarioLogado.Cpf, limite, cursor, gestorExternoPerfilId, _usuarioLogado.OrgId);
            return Ok(perfis);
        }

        [HttpGet("ObterRatecardPorCodigoCliente")]
        public async Task<ActionResult> ObterRatecardPorCodigoCliente(string codCliente)
        {
            var propostas = await _gestaoAlocadosService.ObterRatecardPorCodigoCliente(_usuarioLogado.Cpf, codCliente, _usuarioLogado.OrgId);
            return Ok(propostas);
        }

        [HttpPost("SincronizarClientesEGestoresCRM")]
        public async Task<ActionResult> SincronizarClientesEGestoresCRM()
        {
            var result = await _gestaoAlocadosService.SincronizarClientesEGestoresCRM(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpPost("GetDataUltimaSincronizacaoClientesEGestoresCRM")]
        public async Task<ActionResult> GetDataUltimaSincronizacaoClientesEGestoresCRM()
        {
            var result = await _gestaoAlocadosService.GetDataUltimaSincronizacaoClientesEGestoresCRM(_usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("ListarColaboradoresAlocadosPorCliente")]
        public async Task<ActionResult> ListarColaboradoresAlocadosPorCliente(int limite, int cursor, string busca, string codCliente)
        {
            var clientes = await _gestaoAlocadosService.ListarColaboradoresAlocadosPorCliente(_usuarioLogado.Cpf, limite, cursor, busca, codCliente, _usuarioLogado.OrgId);
            return Ok(clientes);
        }

        [HttpGet("ListarColaboradoresAlocadosPorClienteSemAderencia")]
        public async Task<ActionResult> ListarColaboradoresAlocadosPorClienteSemAderencia(int limite, int cursor, string busca, string codCliente)
        {
            var clientes = await _gestaoAlocadosService.ListarColaboradoresAlocadosPorClienteSemAderencia(_usuarioLogado.Cpf, limite, cursor, busca, codCliente, _usuarioLogado.OrgId);
            return Ok(clientes);
        }

        [HttpGet("ListarColaboradoresAlocadosPorClienteCompleto")]
        public async Task<ActionResult> ListarColaboradoresAlocadosPorClienteCompleto(int limite, int cursor, string codCliente, string codGestorAdm = "", string codGestorOper = "")
        {
            var clientes = await _gestaoAlocadosService.ListarColaboradoresAlocadosPorClienteCompleto(_usuarioLogado.Cpf, limite, cursor, codCliente, _usuarioLogado.OrgId, codGestorAdm, codGestorOper);
            return Ok(clientes);
        }
    }
}