using Colaboracao.Core.Interfaces;
using DataTransferObject.Domain.Usuario;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Colaboracao.Core.Impl
{
    public class AspNetUser : IAspNetUser
    {
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;
        private readonly ITokens _tokens;

        public AspNetUser(Microsoft.AspNetCore.Http.IHttpContextAccessor pHttpContextAccessor, ITokens tokens)
        {
            _httpContextAccessor = pHttpContextAccessor;
            _tokens = tokens;
        }

        public UsuarioLogadoDTO GetUsuarioLogado()
        {
            UsuarioLogadoDTO ret = null;
            if (Claims() != null && Claims().Any())
            {
                var claim = Claims();
                var email = claim.Where(m => m.Type == "Email").FirstOrDefault().Value;
                var cpf = claim.Where(m => m.Type == "Cpf").FirstOrDefault().Value;
                var cod_colaborador = claim.Where(m => m.Type == "CodColaborador").FirstOrDefault().Value;
                var orgId = int.Parse(Claims().Where(m => m.Type == "OrgId").FirstOrDefault().Value);
                var loginType = int.Parse(Claims().Where(m => m.Type == "LoginType").FirstOrDefault().Value);
                var token = _httpContextAccessor.HttpContext.Request.Headers["authorization"].ToString()["Bearer ".Length..].Trim();
                ret = new UsuarioLogadoDTO
                {
                    Token = token,
                    TipoLogin = (TipoLoginEnum)loginType,
                    Cpf = cpf,
                    Email = email,
                    CodColaborador = cod_colaborador,
                    OrgId = orgId
                };
            }
            return ret;
        }

        private IEnumerable<Claim> Claims()
        {
            return _httpContextAccessor.HttpContext?.User?.Claims ?? null;
        }
    }
}