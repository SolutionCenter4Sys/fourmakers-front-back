using System;
using System.Data;
using System.Threading.Tasks;
using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Base;
using DataTransferObject.Domain.Colaborador;
using DataTransferObject.Domain.Colaborador.Campanha._2025_01_COLETA_PERFIL_COLABORADOR;
using DataTransferObject.Domain.Fourmakers;
using DataTransferObject.Domain.Log;
using DataTransferObject.Domain.Usuario;
using Foursys.Domain.Interfaces.Services;
using Logs.Infra.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Questionario.Domain.Interfaces;

namespace Foursys.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [HandleException]
    [LogAction]
    public class FourmakersController : ControllerBase
    {
        private readonly IFourmakersLeadsService _foursmakersLeadLeadService;
        private readonly IFourmakersService _foursmakersService;
        private readonly UsuarioLogadoDTO _usuarioLogado;

        public FourmakersController(IFourmakersLeadsService foursmakersLeadService,IFourmakersService foursmakersService, IAspNetUser aspNetUser)
        {
            _foursmakersLeadLeadService = foursmakersLeadService;
            _foursmakersService = foursmakersService;
            _usuarioLogado = aspNetUser.GetUsuarioLogado();
        }

        [AllowAnonymous]
        [HttpPost("CapturarLead")]
        public async Task<IActionResult> CapturarLead([FromBody] CapturarLeadDTO input)
        {
            return Ok(await _foursmakersLeadLeadService.InserirLeadAsync(input.Nome, input.Email, input.Telefone, input.NomeEmpresa, input.OpcaoColaboradorEnum));
        }
        
        [HttpPost("InserirAvaliacao")]
        public async Task<IActionResult> InserirAvaliacao([FromBody] AvaliacaoFourmakersDTO input)
        {
            return Ok(await _foursmakersService.InserirAvaliacaoAsync(_usuarioLogado.OrgId, _usuarioLogado.Cpf, input.ServicoRate, input.RecomendacaoRate, input.ExperienciaDescricao, input.AspectoDescricao));
        }
    }
}