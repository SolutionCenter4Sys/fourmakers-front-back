using Colaboracao.Core.Impl;
using Colaboracao.Core.Interfaces;
using Colaboracao.Infra.Repositories.Fourmakers;
using Core.Domain.ParametroOrg;
using Foursys.Domain.Impl.Services;
using Foursys.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Colaboracao.Initializer
{
    public static partial class InitializerExtension
    {
        public static void AddBuscaParametroConfiguracaoServicesDependencies(this IServiceCollection services, bool transient = false)
        {
            if (transient)
            {
                services.TryAddTransient<IBuscaParametroConfiguracaoService, BuscaParametroConfiguracaoService>();
                services.TryAddTransient<IParametroConfiguracaoRepository, ParametroConfiguracaoRepository>();
                services.TryAddTransient<IConnectionStringCore, ConnectionStringCore>();
            }
            else
            {
                services.TryAddScoped<IBuscaParametroConfiguracaoService, BuscaParametroConfiguracaoService>();
                services.TryAddScoped<IParametroConfiguracaoRepository, ParametroConfiguracaoRepository>();
                services.TryAddScoped<IConnectionStringCore, ConnectionStringCore>();
            }
        }
    }
}