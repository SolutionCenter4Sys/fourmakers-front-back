using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Financeiro.IntegracaoBancaria.RemessaBancaria;
using DataTransferObject.Domain.Usuario;
using Financeiro.Domain.Interfaces.IntegracaoBancaria.RemessaBancaria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DataTransferObject.Domain.Base;

namespace Financeiro.API.Controllers.IntegracaoBancaria
{
    [Authorize]
    [Route("api/Financeiro/IntegracaoBancaria/[controller]")]
    [HandleException]
    [ApiController]
    public class RemessaBancariaController : ControllerBase
    {
        private readonly IRemessaBancariaService _remessaBancariaService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public RemessaBancariaController(IRemessaBancariaService remessaBancariaService, IAspNetUser aspNetUser)
        {
            _remessaBancariaService = remessaBancariaService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [HttpPost("ProcessarRemessaBancariaCNAB")]
        public async Task<ActionResult<ApiGenericResult<ListarRemessasCnabResult>>> ProcessarRemessa([FromBody] GerarRemessaBancariaRequest request)
        {
            var result = await _remessaBancariaService.ProcessarRemessaBancariaCNAB(request, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("ListarSolicitacoesPagamentoCNAB")]
        public async Task<ActionResult<ApiGenericResult<ListarSolicitacoesPagamentoCNABResult>>> ListarSolicitacoesPagamentoCNAB(string tipoRemessa, string? codDiretoria)
        {
            var result = await _remessaBancariaService.ListarSolicitacoesPagamentoCNAB(tipoRemessa, codDiretoria, _usuarioLogado.Cpf, _usuarioLogado.OrgId);
            return Ok(result);
        }

        [HttpGet("ListarRemessasCnab")]
        public async Task<ActionResult<ApiGenericResult<ListarRemessasCnabResult>>> ListarRemessasCnab(string tipoRemessa, string? mesAnoProcessamento = null, string? status = null)
        {
            int orgId = _usuarioLogado.OrgId;
            var result = await _remessaBancariaService.ListarRemessasCnab(orgId,tipoRemessa, mesAnoProcessamento, status);
            return Ok(result);
        }
        
        [HttpGet("BuscarModulosRemessa")]
        public async Task<ActionResult<ApiGenericResult<List<string>>>> BuscarModulosRemessa()
        {
            int orgId = _usuarioLogado.OrgId;
            string cpfRequest = _usuarioLogado.Cpf;
            var result = await _remessaBancariaService.BuscarModulosRemessaOrg(orgId, cpfRequest);
            return Ok(result);
        }
    }
}
