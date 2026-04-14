using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Usuario;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organograma.Domain.Interfaces;
using System.Collections.Generic;

namespace Organograma.API.Controllers
{
    [HandleException]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [LogAction]
    public class OrganogramaController : ControllerBase
    {
        private readonly IOrganogramaService _organogramaService;
        UsuarioLogadoDTO _usuarioLogado;

        public OrganogramaController(IOrganogramaService organogramaService, IAspNetUser aspNetUser)
        {
            _organogramaService = organogramaService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        // ------------Departamento

        [HttpPost("DepartamentoInserir")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaDeptoListarPorIdResponseDTO>>> DepartamentoInserir([FromBody] OrganogramaDeptoInserirParamDTO param)
           => Ok(await _organogramaService.InserirDepartamento(param));

        [HttpPost("DepartamentoAtualizar")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaDeptoListarPorIdResponseDTO>>> DepartamentoAtualizar([FromBody] OrganogramaDeptoAtualizarParamDTO param)
            => Ok(await _organogramaService.AtualizarDepartamento(param));

        [HttpDelete("DepartamentoDeletar")]
        public async Task<ActionResult<ApiGenericResult<bool>>> DepartamentoDeletar(string departamentotoId)
            => Ok(await _organogramaService.DeletarDepartamento(departamentotoId));


        [HttpGet("DepartamentoListaPorId")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaDeptoListarPorIdResponseDTO>>> DepartamentoListaPorId(string buscaId)
            => Ok(await _organogramaService.ListaDepartamentoPorId(buscaId));

        [HttpGet("DepartamentoListarPorCliente")]
        public async Task<ActionResult<ApiGenericResult<List<OrganogramaDeptoListarPorIdResponseDTO>>>> DepartamentoListarPorCliente(string codCliente)
            => Ok(await _organogramaService.ListarDepartamentoPorCliente(codCliente));

        // ------------Posição


        [HttpPost("PosicaoInserir")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPosicaoListarPorIdResponseDTO>>> PosicaoInserir([FromBody] OrganogramaPosicaoInserirParamDTO param)
            => Ok(await _organogramaService.PosicaoInserir(param));

        [HttpPost("PosicaoAtualizar")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPosicaoListarPorIdResponseDTO>>> PosicaoAtualizar([FromBody] OrganogramaPosicaoAtualizarParamDTO param)
            => Ok(await _organogramaService.PosicaoAtualizar(param));

        [HttpDelete("PosicaoDeletar")]
        public async Task<ActionResult<ApiGenericResult<bool>>> PosicaoDeletar(string posicaoId)
            => Ok(await _organogramaService.PosicaoDeletar(posicaoId));

        [HttpGet("PosicaoListaPorId")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPosicaoListarPorIdResponseDTO>>> PosicaoListaPorId(string buscaId)
            => Ok(await _organogramaService.PosicaoListaPorId(buscaId));

        [HttpGet("PosicaoListarPorCliente")]
        public async Task<ActionResult<ApiGenericResult<List<OrganogramaPosicaoListarPorIdResponseDTO>>>> PosicaoListarPorCliente(string codCliente)
            => Ok(await _organogramaService.PosicaoListarPorCliente(codCliente));

        // ------------Alocação

        [HttpPost("AlocacaoInserir")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaAlocacaoListarPorIdResponseDTO>>> AlocacaoInserir([FromBody] OrganogramaAlocacaoInserirParamDTO param)
            => Ok(await _organogramaService.AlocacaoInserir(param));

        [HttpPost("AlocacaoAtualizar")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaAlocacaoListarPorIdResponseDTO>>> AlocacaoAtualizar([FromBody] OrganogramaAlocacaoAtualizarParamDTO param)
            => Ok(await _organogramaService.AlocacaoAtualizar(param));

        [HttpDelete("AlocacaoDeletar")]
        public async Task<ActionResult<ApiGenericResult<bool>>> AlocacaoDeletar(string alocacaoId)
            => Ok(await _organogramaService.AlocacaoDeletar(alocacaoId));

        [HttpGet("AlocacaoListaPorId")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaAlocacaoListarPorIdResponseDTO>>> AlocacaoListaPorId(string buscaId)
            => Ok(await _organogramaService.AlocacaoListaPorId(buscaId));

        // ------------Perfil Corporativo Alocação

        [HttpPost("PerfilCorporativoAlocacaoInserir")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO>>> PerfilCorporativoAlocacaoInserir([FromBody] OrganogramaPerfilCorporativoAlocacaoInserirParamDTO param)
            => Ok(await _organogramaService.InserirPerfilCorporativoAlocacao(param));

        [HttpPost("PerfilCorporativoAlocacaoAtualizar")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO>>> PerfilCorporativoAlocacaoAtualizar([FromBody] OrganogramaPerfilCorporativoAlocacaoAtualizarParamDTO param)
            => Ok(await _organogramaService.AtualizarPerfilCorporativoAlocacao(param));

        [HttpDelete("PerfilCorporativoAlocacaoDeletar")]
        public async Task<ActionResult<ApiGenericResult<bool>>> PerfilCorporativoAlocacaoDeletar(string alocacaoId)
            => Ok(await _organogramaService.DeletarPerfilCorporativoAlocacao(alocacaoId));

        [HttpGet("PerfilCorporativoAlocacaoListaPorId")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPerfilCorporativoAlocacaoListarPorIdResponseDTO>>> PerfilCorporativoAlocacaoListaPorId(string buscaId)
            => Ok(await _organogramaService.BuscarPerfilCorporativoAlocacaoPorId(buscaId));

        [HttpGet("ListarColaboradoresExternosAlocados")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<OrganogramaColaboradorAlocadoDTO>>>> ListarColaboradoresExternosAlocados([FromQuery] int limit = 10, [FromQuery] int cursor = 0, [FromQuery] string nome = null)
            => Ok(new ApiGenericResult<IEnumerable<OrganogramaColaboradorAlocadoDTO>> { Retorno = await _organogramaService.ListarColaboradoresExternosAlocados(limit, cursor, nome) });

        [HttpGet("ListarColaboradoresExternos")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<OrganogramaColaboradorAlocadoDTO>>>> ListarColaboradoresExternos([FromQuery] int limit = 10, [FromQuery] int cursor = 0, [FromQuery] string nome = null)
            => Ok(new ApiGenericResult<IEnumerable<OrganogramaColaboradorAlocadoDTO>> { Retorno = await _organogramaService.ListarColaboradoresExternos(limit, cursor, nome) });

        [HttpGet("ListarColaboradoresExternosPorCliente")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<OrganogramaColaboradorAlocadoDTO>>>> ListarColaboradoresExternosPorCliente([FromQuery] string codigoCliente, [FromQuery] int limit = 10, [FromQuery] int cursor = 0, [FromQuery] string nome = null)
            => Ok(new ApiGenericResult<IEnumerable<OrganogramaColaboradorAlocadoDTO>> { Retorno = await _organogramaService.ListarColaboradoresExternosPorCliente(codigoCliente, limit, cursor, nome) });

        // ------------Perfil Corporativo

        [HttpPost("PerfilCorporativoInserir")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPerfilCorporativoListarPorIdResponseDTO>>> PerfilCorporativoInserir([FromBody] OrganogramaPerfilCorporativoInserirParamDTO param)
            => Ok(await _organogramaService.PerfilCorporativoInserir(param));

        [HttpPost("PerfilCorporativoAtualizar")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPerfilCorporativoListarPorIdResponseDTO>>> PerfilCorporativoAtualizar([FromBody] OrganogramaPerfilCorporativoAtualizarParamDTO param)
            => Ok(await _organogramaService.PerfilCorporativoAtualizar(param));

        [HttpDelete("PerfilCorporativoDeletar")]
        public async Task<ActionResult<ApiGenericResult<bool>>> PerfilCorporativoDeletar(string perfilCorpId)
            => Ok(await _organogramaService.PerfilCorporativoDeletar(perfilCorpId));

        [HttpGet("PerfilCorporativoListaPorId")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPerfilCorporativoListarPorIdResponseDTO>>> PerfilCorporativoListaPorId(string perfilCorpId)
            => Ok(await _organogramaService.PerfilCorporativoListaPorId(perfilCorpId));

        [HttpGet("BuscarPerfisPorOrg")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<OrganogramaPerfilCorporativoListarPorIdResponseDTO>>>> BuscarPerfisPorOrg(int orgId)
            => Ok(await _organogramaService.BuscarPerfisPorOrgId(orgId));

        // ------------Perfil Corporativo Skill

        [HttpPost("PerfilCorporativoSkillInserir")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO>>> PerfilCorporativoSkillInserir([FromBody] OrganogramaPerfilCorporativoSkillInserirParamDTO param)
            => Ok(await _organogramaService.PerfilCorporativoSkillInserir(param));

        [HttpPost("PerfilCorporativoSkillAtualizar")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPerfilCorporativoSkillListarPorIdResponseDTO>>> PerfilCorporativoSkillAtualizar([FromBody] OrganogramaPerfilCorporativoSkillAtualizarParamDTO param)
            => Ok(await _organogramaService.PerfilCorporativoSkillAtualizar(param));

        [HttpDelete("PerfilCorporativoSkillDeletar")]
        public async Task<ActionResult<ApiGenericResult<bool>>> PerfilCorporativoSkillDeletar(string perfilCorpSkillId)
            => Ok(await _organogramaService.PerfilCorporativoSkillDeletar(perfilCorpSkillId));

        [HttpGet("PerfilCorporativoSkillListaPorId")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaPerfilCorporativoListarPorIdResponseDTO>>> PerfilCorporativoSkillListaPorId(string perfilCorpSkillId)
            => Ok(await _organogramaService.PerfilCorporativoSkillListaPorId(perfilCorpSkillId));

        // ------------Clientes Org
        [HttpGet("RetornarClientesPorOrgId")]
        public async Task<ActionResult<ApiGenericResult<IEnumerable<OrganogramaClienteOrgDTO>>>> RetornarClientesPorOrgId(int limite, int cursor, string busca = null)
            => Ok(await _organogramaService.RetornarClientesPorOrgId(_usuarioLogado.OrgId, limite, cursor, busca));

        // ------------Organograma Completo
        [HttpGet("OrganogramaCompletoPorCliente")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaCompletoResponseDTO>>> OrganogramaCompletoPorCliente(string codigoCliente)
            => Ok(await _organogramaService.RetornarOrganogramaCompletoPorCliente(codigoCliente));

        [HttpGet("OrganogramaCompletoPorClienteCLevel")]
        public async Task<ActionResult<ApiGenericResult<OrganogramaCompletoResponseDTO>>> OrganogramaCompletoPorClienteCLevel(string codigoCliente)
            => Ok(await _organogramaService.RetornarOrganogramaCompletoPorClienteCLevel(codigoCliente));


    }
}
