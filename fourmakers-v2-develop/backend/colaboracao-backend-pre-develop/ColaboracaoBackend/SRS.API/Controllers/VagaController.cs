using Colaboracao.Core;
using Colaboracao.Core.Interfaces;
using Logs.Infra.Attributes;
using MapaDeAlocacao.Domain.Interfaces.Perfil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SRS.Domain.Interfaces.Service;

namespace SRS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [LogAction]
    public partial class VagaController : ControllerBase
    {
        private readonly IVagaService _vagaService;
        private readonly IAspNetUser _aspNetUser;
        private readonly ILogCore _log;
        private readonly IConfiguration _configuration;
        private readonly IPerfilService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenSistemaService _tokenSistemaService;

        public VagaController(IVagaService vagaService, IAspNetUser aspNetUser, ILogCore log, IConfiguration configuration, IPerfilService service,
            IHttpContextAccessor httpContextAccessor, ITokenSistemaService tokenSistemaService)
        {
            _vagaService = vagaService;
            _aspNetUser = aspNetUser;
            _log = log;
            _configuration = configuration;
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _tokenSistemaService = tokenSistemaService;
        }

        // ATENÇÃO: Este controller utiliza a abordagem de classes parciais para organizar os métodos.
        // Certifique-se de verificar as outras partes da classe antes de implementar ou modificar métodos neste arquivo.
    }
}