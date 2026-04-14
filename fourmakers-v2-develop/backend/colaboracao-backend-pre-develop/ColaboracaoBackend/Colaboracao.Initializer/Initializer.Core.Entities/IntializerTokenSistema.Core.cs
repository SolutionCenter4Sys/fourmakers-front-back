using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Repositories.TemplateEmail;
using Colaboracao.Infra.Repositories.Usuario;
using Core.Domain.TemplateEmail;
using Core.Domain.Usuario;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TemplateOrg.Impl;
using TemplateOrg.Interfaces;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddTokenSistemaDependencies(this IServiceCollection services, bool transient = false)
        {
            if (transient)
            {
                services.TryAddTransient<ITokenSistemaService, TokenSistemaService>();
                services.TryAddTransient<ITokenSistemaRepository, TokenSistemaRepository>();
            }
            else
            {
                services.TryAddScoped<ITokenSistemaService, TokenSistemaService>();
                services.TryAddScoped<ITokenSistemaRepository, TokenSistemaRepository>();
            }
        }
    }
}