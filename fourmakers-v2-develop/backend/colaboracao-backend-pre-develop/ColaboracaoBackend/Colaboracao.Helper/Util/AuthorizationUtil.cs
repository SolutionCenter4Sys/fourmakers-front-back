using Microsoft.AspNetCore.Http;
using System;

namespace Colaboracao.Helper.Util;

public static class AuthorizationUtil
{
    /// <summary>
    /// Extrai o token Bearer do header Authorization
    /// </summary>
    /// <param name="httpContext">Contexto HTTP</param>
    /// <returns>Token extraído do header</returns>
    /// <exception cref="UnauthorizedAccessException">Lançada quando o header Authorization não está presente ou inválido</exception>
    public static string ObterTokenBearer(HttpContext httpContext)
    {
        if (httpContext?.Request?.Headers == null)
        {
            throw new UnauthorizedAccessException("Contexto HTTP inválido");
        }

        var authorizationHeader = httpContext.Request.Headers["authorization"].ToString();

        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            throw new UnauthorizedAccessException("Header Authorization não encontrado");
        }

        if (!authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Header Authorization inválido. Formato esperado: Bearer {token}");
        }

        var token = authorizationHeader["Bearer ".Length..].Trim();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedAccessException("Token não encontrado no header Authorization");
        }

        return token;
    }
}