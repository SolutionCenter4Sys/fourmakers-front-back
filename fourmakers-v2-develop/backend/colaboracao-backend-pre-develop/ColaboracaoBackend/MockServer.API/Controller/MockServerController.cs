using ApiClient.Domain.Interfaces;
using Colaboracao.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MockServer.Domain.Interfaces.Services;
using System;

namespace MockServer.API.Controller
{
    [Route("Validacao.API/api/Recurso/Gerentes/")]
    [ApiController]
    public class MockServerController : ControllerBase
    {
        private readonly IMockServerService _mockServerService;
        private readonly ICCHClient _cchClient;
        private readonly IAspNetUser _aspNetUser;

        public MockServerController(IMockServerService mockServerService, IAspNetUser aspNetUser, ICCHClient cCHClient)
        {
            _mockServerService = mockServerService;
            _aspNetUser = aspNetUser;
            _cchClient = cCHClient;
        }

        [HttpGet("Hierarquia")]
        public ActionResult<string> HierarquiaMock()
        {
            try
            {
                return _mockServerService.HierarquiaMock();
            }
            catch (Exception e)
            {
                return StatusCode(401, e.Message);
            }
        }
    }
}