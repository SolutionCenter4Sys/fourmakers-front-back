using ApiClient.Domain;
using Colaboracao.Helper;
using DataTransferObject.Domain.Usuario;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Colaboracao.Core.Impl
{
    public static class JWTAuth
    {
        public static void ConfigureJWT(IServiceCollection services)
        {
            var key = Encoding.ASCII.GetBytes(VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.AUTH_SECRET));
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = TokenSistemaValidadao
                };
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        private static async Task TokenSistemaValidadao(MessageReceivedContext context)
        {
            var authorization = context.HttpContext.Request.Headers["authorization"].ToString();
            var bearer = "Bearer ";

            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith(bearer, StringComparison.OrdinalIgnoreCase))
            {
                var token = authorization[bearer.Length..].Trim();

                if (!string.IsNullOrEmpty(token) && token == VariaveisDeAmbienteUtil.GetVariavelDeAmbiente(EnvironmentVariables.TOKEN_SISTEMA_COLABORACAO))
                {
                    var claims = new[] {
                    new Claim("Email", "admin@admin.com"),
                    new Claim("Cpf", "00000000000"),
                    new Claim("CodColaborador", ""),
                    new Claim("OrgId", "1"),
                    new Claim("LoginType", ((int)TipoLoginEnum.SISTEMA).ToString()),
                };
                    var identity = new ClaimsIdentity(claims, "AdminAccess");
                    context.Principal = new ClaimsPrincipal(identity);
                    context.Success();
                }
            }
        }
    }
}